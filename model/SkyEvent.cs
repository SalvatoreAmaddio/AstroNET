using Backend.Database;
using FrontEnd.Controller;
using MvvmHelpers;
using NodaTime;
using SwissEphNet;
using System.ComponentModel;

namespace WpfApp1.model
{
    public enum SkyType 
    { 
        Sky = 1,
        SunReturn = 2,
        MoonReturn = 3,
        Horoscope = 4,
        Sinastry = 5,
    }

    public class FeaturesCount
    {
        public string Name { get; private set; }
        public int Count { get; private set; }

        public FeaturesCount(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override string? ToString() => $"{Name} - {Count}";
    }

    public class SkyEvent : INotifyPropertyChanged
    {
        public SkyType SkyType { get; private set; } = SkyType.Sky;

        public event PropertyChangedEventHandler? PropertyChanged;
        public double JulianDay { get; private set; }
        public double HourUT { get; private set; }
        public TimeSpan UT
        {
            get
            {
                int hours = (int)Math.Floor(HourUT);
                int minutes = (int)((HourUT - hours) * 60);
                return new TimeSpan(hours, minutes, 0);
            }
        }
        public TimeSpan LocalTime { get; private set; }
        public DateTime LocalDateTime { get; set; }
        public double LocalHour { get; private set; }
        public City City { get; private set; } = null!;
        public int Year => LocalDateTime.Year;
        public int Month => LocalDateTime.Month;
        public int Day => LocalDateTime.Day;
        public double SideralTime { get; private set; }
        public ObservableRangeCollection<Star> Stars { get; private set; } = [];
        public int FireSigns => Stars.Count(s=>s.RadixSign.Element.ElementId == 1);
        public int WaterSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 2);
        public int AirSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 3);
        public int EarhSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 4);
        public int MaleSigns => Stars.Count(s => s.RadixSign.Gender.GenderId == 1);
        public IEnumerable<FeaturesCount>? HousesCount 
        { 
            get 
            {
                try 
                { 
                    return Stars.GroupBy(s => s.House).Select(s => new FeaturesCount(s.Key?.PointName ?? "N/A", s.Count())).OrderByDescending(s => s.Count);
                }
                catch 
                { 
                    return null;
                }
            }
        }

        public int FemaleSigns => Stars.Count(s => s.RadixSign.Gender.GenderId == 2);
        public int CardinalSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 3);
        public int FixedSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 1);
        public int MobileSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 2);
        public ObservableRangeCollection<House> Houses { get; } = [];
        public List<IPoint> Aspectables { get; } = [];
        public SkyEvent? Horoscope { get; private set; }
        public ObservableRangeCollection<Aspect> RadixAspects { get; } = [];

        private List<Aspect> Aspects = new(DatabaseManager.Find<Aspect>()!.MasterSource.Cast<Aspect>());
        public bool ShowHouses { get; set; } = true;
        public IAbstractFormController? PersonController { get; private set; }
        public Person? Person { get; private set; }

        public SkyEvent() { }
        public SkyEvent(DateTime? date)
        {
        }

        public SkyEvent(Person person, IAbstractFormController controller) : this(person)
        {
            PersonController = controller;
        }

        public SkyEvent(Person person) : this(person.DOB, person.TOB, person.City, !person.UnknownTime)
        {
            Person = person;
        }

        public SkyEvent(Person person, bool showHouses) : this(person.DOB, person.TOB, person.City, showHouses)
        {
            Person = person;
        }

        public SkyEvent(DateTime date, TimeSpan time, City city, bool showHouses = true) : this()
        { 
            Calculate(date, time, city);
            ShowHouses = showHouses;
            CalculatePositions();
        }

        public SkyEvent(int year, int month, int day, int hour, int minutes, City city, bool showHouses = true) : this()
        { 
            Calculate(year, month, day, hour, minutes, city);
            ShowHouses = showHouses;
            CalculatePositions();
        }

        public void Calculate(DateTime date, TimeSpan time, City city) =>
        Calculate(date.Year, date.Month, date.Day, time.Hours, time.Minutes, city);

        private static bool CheckDST(DateTime dateTime, string timeZoneId)
        {
            // Convert DateTime to NodaTime LocalDateTime
            LocalDateTime localDateTime = new (dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);

            // Get the time zone from the IANA Time Zone Database
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

            // Convert LocalDateTime to ZonedDateTime
            ZonedDateTime zonedDateTime = timeZone.AtLeniently(localDateTime);

            // Check if the date is in DST
            return zonedDateTime.IsDaylightSavingTime();           
        }

        public void Calculate(int year, int month, int day, int hour, int minutes, City city)
        {
            City = city;
            City.Build();
            LocalHour = hour + minutes / 60.0;
            LocalTime = new(hour, minutes, 0);
            LocalDateTime = new DateTime(year, month, day, (int)LocalHour, (int)((LocalHour - (int)LocalHour) * 60), 0);

            bool isDaylight = CheckDST(LocalDateTime,City.TimeZone.TimeZoneName);

            TimeSpan timeOffset = City.TimeZone.Info.BaseUtcOffset;

            if (isDaylight)
                timeOffset = timeOffset.Add(new TimeSpan(1, 0, 0));

            HourUT = LocalHour - timeOffset.TotalHours;
            CalculateJulianDate();
        }

        private void CalculateJulianDate()
        {
            SwissEph sw = new();
            JulianDay = sw.swe_julday(Year, Month, Day, HourUT, SwissEph.SE_GREG_CAL);
            SideralTime = sw.swe_sidtime(JulianDay) + (City.Longitude / 15.0);
            if (SideralTime < 0) SideralTime += 24;
        }

        public void CalculatePositions()
        {
            PositionCalculator positionCalculator = new(this);

            positionCalculator.CalculateHouses();

            for (int i = 0; i < 10; i++) 
            {
                Stars.Add(positionCalculator.CalculatePlanet(i, !ShowHouses));
                Stars[i].Build();
            }

            for (int i = 1; i <= 12; i++) 
            {
                Houses.Add(positionCalculator.GetHouse(i));
                Houses[i-1].Build();
            }

            Aspectables.AddRange(Stars);

            if (ShowHouses)
                Aspectables.AddRange(Houses);

            CalculateBirthAspects();
        }

        private void AddRadixAspect(Aspect? aspect, IPoint pointA, IPoint pointB)
        {
            if (aspect != null)
            {
                aspect.PointA = pointA;
      
                pointB.Build();
                
                aspect.PointB = pointB;

                aspect.CalculateOrbDiff();
                RadixAspects.Add(aspect);
            }
        }

        private void CalculateBirthAspects()
        {
            List<IPoint> aspectables = new(this.Aspectables);
            RadixAspects.Clear();

            foreach (IStar star in Stars)
            {
                foreach (IPoint pointReceiver in aspectables)
                {
                    if (star.PointName.Equals(pointReceiver.PointName)) continue;

                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);
                
                    foreach (Aspect aspect in Aspects)
                    {
                        if (pointReceiver is IHouse house && !house.IsAngular && aspect.Orbit != 0)
                            continue;

                        Aspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance);

                        if (calculatedAspect != null)
                        {
                            if (pointReceiver is IHouse house2 && !house2.IsAngular && calculatedAspect.Orbit == 0)
                            {
                                if (distance > 2.5 || distance < -2.5) 
                                {
                                    continue;
                                }
                            }
                            calculatedAspect.DateOf = LocalDateTime;
                            AddRadixAspect(calculatedAspect, star, pointReceiver);
                        }
                    }
                }

                aspectables.Remove(star);
            }
        }

        public SkyEvent CalculateReturn(DateTime returnDate, TimeSpan returnTime, City selectedCity, SkyType skyType = SkyType.SunReturn)
        {
            SkyEvent returnSky = new(returnDate, returnTime, selectedCity) 
            {
                SkyType = skyType,
                Person = Person,
            };

            returnSky.RadixAspects.ReplaceRange(returnSky.RadixAspects.Where(s=>s.Orbit == 0 && s.PointB is IHouse && (s.OrbDiff >=-2.5 && s.OrbDiff <= 2.5)).ToList());

            House returnAsc = returnSky.Houses.First();

            returnAsc.PointName = $"R. {returnAsc.PointName}";

            Aspect conj = Aspects.First(s=>s.Orbit == 0);

            foreach(House radixHouse in Houses) 
            {
                double dist = PositionCalculator.CalculateDistance(returnAsc.EclipticLongitude, radixHouse.EclipticLongitude);
                Aspect? asp = PositionCalculator.IsValidAspect(conj, dist, 2.5);
                if (asp == null) continue;
                radixHouse.PointName = $"Radix {radixHouse.PointName}";
                asp.DateOf = returnDate;
                returnSky.AddRadixAspect(asp, returnAsc, radixHouse);
            }
            
            return returnSky;
        }

        public void CalculateHoroscope(DateTime date, TimeSpan time, City city)
        {
            IEnumerable<StarTransitOrbit> transitAspects = DatabaseManager.Find<StarTransitOrbit>()!.MasterSource.Cast<StarTransitOrbit>();

            Horoscope = new(date, time, city, false) 
            {
                SkyType = SkyType.Horoscope,
                Person = Person,
            };

            Horoscope.RadixAspects.Clear();
           
            foreach (IStar star in Horoscope.Stars) 
            { 
                if (Person!=null && !Person.UnknownTime)
                    star.PlaceInHouse(this);
                   
                foreach (IPoint pointReceiver in this.Aspectables)
                {
                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    foreach (Aspect aspect in Aspects)
                    {
                        double? tollerance = transitAspects.FirstOrDefault(s => s.Aspect.Equals(aspect) && s.Star.Equals(star))?.Tollerance;

                        if (tollerance == null) continue;
                        
                        if (pointReceiver is IHouse house && !house.IsAngular && aspect.Orbit != 0)
                            continue;

                        Aspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                        if (calculatedAspect != null) 
                        {
                            if (pointReceiver is IHouse house2 && !house2.IsAngular)
                            {
                                if (distance > 2.5 || distance < -2.5)
                                    continue;
                            }

                            calculatedAspect.TransitType = new(2);
                            calculatedAspect.DateOf = date;
                            Horoscope.AddRadixAspect(calculatedAspect, star, pointReceiver);
                        }
                    }
                }
            }
        }

        public IEnumerable<Aspect> CalculateStarAspects(Star star, DateTime date)
        {
            IEnumerable<StarTransitOrbit> transitAspects = DatabaseManager.Find<StarTransitOrbit>()!.MasterSource.Cast<StarTransitOrbit>();

            if (Person != null && !Person.UnknownTime)
                star.PlaceInHouse(this);

            foreach (IPoint pointReceiver in this.Aspectables)
            {
                double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                foreach (Aspect aspect in Aspects)
                {
                    double? tollerance = transitAspects.FirstOrDefault(s => s.Aspect.Equals(aspect) && s.Star.Equals(star))?.Tollerance;
                    if (tollerance == null) continue;

                    if (pointReceiver is IHouse house && !house.IsAngular && aspect.Orbit != 0)
                        continue;

                    Aspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                    if (calculatedAspect != null)
                    {
                        if (pointReceiver is IHouse house2 && !house2.IsAngular)
                        {
                            if (distance > 2.5 || distance < -2.5)
                                continue;
                        }

                        calculatedAspect.TransitType = new(2);

                        calculatedAspect.DateOf = date;

                        star.Build();
                        calculatedAspect.PointA = star;

                        pointReceiver.Build();
                        calculatedAspect.PointB = pointReceiver;

                        calculatedAspect.CalculateOrbDiff();
                        yield return calculatedAspect;
                    }
                }
            }
        }

        public void ClearHoroscope() => Horoscope = null;
        public override string ToString()
        {
            return $"{Day:00}/{Month:00}/{Year} at {LocalTime.ToString(@"hh\:mm")} (Location: {City.CityName}, {City.Region.Country} - Lat: {City.Latitude}°, Long: {City.Longitude}°)";
        }
    }
}