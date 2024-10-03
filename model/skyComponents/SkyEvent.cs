using Backend.Database;
using FrontEnd.Controller;
using MvvmHelpers;
using System.ComponentModel;

namespace AstroNET.model
{
    public abstract class AbstractSkyEvent : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public SkyInfo SkyInfo { get; private set; } = new();
        public ObservableRangeCollection<Star> Stars { get; private set; } = [];
        public IEnumerable<ElementGroupKey>? OccupiedHouses =>
        (Houses.Count == 0) ? [] : Stars.GroupBy(s => s.House)
                                        .Select(s => new ElementGroupKey(s.Key, s.Count()))
                                        .OrderByDescending(s => s.Count).ToList();

        public IEnumerable<ElementGroupKey>? Stelliums => OccupiedHouses?.Where(s => s.Count >= 3).ToList();
        public ObservableRangeCollection<House> Houses { get; protected set; } = [];
        public IAbstractFormController? PersonController { get; protected set; }
        public Person? Person { get; protected set; }
        public int FireSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 1);
        public int WaterSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 2);
        public int AirSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 3);
        public int EarhSigns => Stars.Count(s => s.RadixSign.Element.ElementId == 4);
        public int MaleSigns => Stars.Count(s => s.RadixSign.Gender.GenderId == 1);
        public int FemaleSigns => Stars.Count(s => s.RadixSign.Gender.GenderId == 2);
        public int CardinalSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 3);
        public int FixedSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 1);
        public int MobileSigns => Stars.Count(s => s.RadixSign.Triplicity.TriplicityId == 2);

        public List<IPoint> Aspectables { get; } = [];
        public ObservableRangeCollection<Aspect> RadixAspects { get; } = [];

        protected List<Aspect> Aspects = new(DatabaseManager.Find<Aspect>()!.MasterSource.Cast<Aspect>());
        #endregion

        public AbstractSkyEvent() { }
        public AbstractSkyEvent(DateTime date, TimeSpan time, City city)
        {
            SkyInfo.Calculate(date, time, city);
            CalculatePositions();
        }

        protected virtual void CalculatePositions()
        {
            PositionCalculator positionCalculator = new(this);

            positionCalculator.CalculateHouses();

            for (int i = 0; i < 10; i++)
            {
                Stars.Add(positionCalculator.CalculatePlanet(i));
                Stars[i].Build();
            }

            Aspectables.AddRange(Stars);

            for (int i = 1; i <= 12; i++)
            {
                Houses.Add(positionCalculator.GetHouse(i));
                Houses[i - 1].Build();
            }

            Aspectables.AddRange(Houses);

            CalculateBirthAspects();
        }

        protected virtual void CalculateBirthAspects()
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
                            calculatedAspect.DateOf = SkyInfo.LocalDateTime;
                            AddRadixAspect(calculatedAspect, star, pointReceiver);
                        }
                    }
                }

                aspectables.Remove(star);
            }
        }

        protected void AddRadixAspect(Aspect? aspect, IPoint pointA, IPoint pointB)
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
    }

    public class SkyEvent : AbstractSkyEvent
    {
        public bool ShowHouses { get; set; } = true;
        public SkyEvent? Horoscope { get; private set; }

        #region Constructors
        public SkyEvent() { }

        public SkyEvent(Person person, IAbstractFormController controller) : this(person)
        {
            PersonController = controller;
        }

        public SkyEvent(Person person) : this(person.DOB, person.TOB, person.City, !person.UnknownTime)
        {
            Person = person;
        }

        public SkyEvent(Person person, bool showHouses)
            : this(person.DOB, person.TOB, person.City, showHouses) =>
            Person = person;

        public SkyEvent(DateTime date, TimeSpan time, City city, bool showHouses = true) : base(date, time, city)
        {
            ShowHouses = showHouses;
            CalculatePositions();
        }

        public SkyEvent(int year, int month, int day, int hour, int minutes, City city, bool showHouses = true) : this()
        {
            ShowHouses = showHouses;
            SkyInfo.Calculate(year, month, day, hour, minutes, city);
            CalculatePositions();
        }
        #endregion

        protected override void CalculatePositions()
        {
            Houses.Clear();
            Stars.Clear();
            Aspectables.Clear();

            PositionCalculator positionCalculator = new(this);

            positionCalculator.CalculateHouses();

            for (int i = 0; i < 10; i++)
            {
                Stars.Add(positionCalculator.CalculatePlanet(i, !ShowHouses));
                Stars[i].Build();
            }

            Aspectables.AddRange(Stars);

            if (ShowHouses)
            {
                for (int i = 1; i <= 12; i++)
                {
                    Houses.Add(positionCalculator.GetHouse(i));
                    Houses[i - 1].Build();
                }

                Aspectables.AddRange(Houses);
            }

            CalculateBirthAspects();
        }

        public ReturnSkyEvent CalculateReturn(DateTime returnDate, TimeSpan returnTime, City selectedCity, SkyType skyType = SkyType.SunReturn) =>
        new(returnDate, returnTime, selectedCity, this, skyType);

        public SkyEvent CloneMe() =>
        new(SkyInfo.Year, SkyInfo.Month, SkyInfo.Day, SkyInfo.LocalTime.Hours, SkyInfo.LocalTime.Minutes, SkyInfo.City, ShowHouses) { Person = this.Person };

        private static void CompleteAspect(ref Aspect aspect, IPoint pointA, IPoint pointB)
        {
            aspect.PointA = pointA;

            pointB.Build();

            aspect.PointB = pointB;

            aspect.CalculateOrbDiff();
        }

        public void CalculateHoroscope(DateTime date, TimeSpan time, City city)
        {
            IEnumerable<StarTransitOrbit> transitAspects = DatabaseManager.Find<StarTransitOrbit>()!.MasterSource.Cast<StarTransitOrbit>();

            Horoscope = new(date, time, city, false)
            {
                Person = Person,
                Houses = this.Houses
            };

            Horoscope.SkyInfo.SkyType = SkyType.Horoscope;
            Horoscope.RadixAspects.Clear();

            foreach (IStar star in Horoscope.Stars)
            {
                if (Person != null && !Person.UnknownTime)
                    star.PlaceInHouse(this);

                foreach (IPoint pointReceiver in this.Aspectables)
                {
                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    foreach (Aspect aspect in Aspects)
                    {
                        double? tollerance = transitAspects.FirstOrDefault(s => s.Aspect.Equals(aspect) && s.Star.Equals(star))?.Tollerance;

                        if (tollerance == null) continue;

                        if (pointReceiver is IHouse house && !house.IsAngular && !aspect.IsConjunction)
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

        public IEnumerable<Star>? StarsInPartnerHouses(SkyEvent match)
        {
            if (match.Person.UnknownTime) return null;
            PositionCalculator positionCalculator = new(this);

            List<Star> stars = [];

            for (int i = 0; i < 10; i++)
            {
                stars.Add(positionCalculator.CalculatePlanet(i, !ShowHouses));
                stars[i].Build();
                stars[i].PlaceInHouse(match);
            }

            return stars;
        }

        public IEnumerable<Aspect> CalculateSinastry(SkyEvent match)
        {
            List<Aspect> aspects = [];
            IEnumerable<StarTransitOrbit> transitAspects = DatabaseManager.Find<StarTransitOrbit>()!.MasterSource.Cast<StarTransitOrbit>();

            foreach (IStar star in Stars)
            {
                foreach (IPoint pointReceiver in match.Aspectables)
                {
                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    foreach (Aspect aspect in Aspects)
                    {
                        double? tollerance = transitAspects.FirstOrDefault(s => s.Aspect.Equals(aspect) && s.Star.Equals(star))?.Tollerance;

                        if (tollerance == null) continue;

                        if (pointReceiver is IHouse house && !house.IsAngular && !aspect.IsConjunction)
                            continue;

                        Aspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                        if (calculatedAspect != null)
                        {
                            if (pointReceiver is IHouse house2 && !house2.IsAngular)
                            {
                                if (distance > 2.5 || distance < -2.5)
                                    continue;
                            }

                            calculatedAspect.TransitType = new(3);
                            CompleteAspect(ref calculatedAspect, star, pointReceiver);
                            aspects.Add(calculatedAspect);
                        }
                    }
                }
            }

            return aspects;
        }

        public async Task<SinastryBundle> CalculateSinastryAsync(SkyEvent match)
        {
            Task<IEnumerable<Aspect>> sinastryAspect1Task = Task.Run(() => CalculateSinastry(match));
            Task<IEnumerable<Aspect>> sinastryAspect2Task = Task.Run(() => match.CalculateSinastry(this));
            Task<IEnumerable<Star>?> sinastryStars1 = Task.Run(() => StarsInPartnerHouses(match));
            Task<IEnumerable<Star>?> sinastryStars2 = Task.Run(() => match.StarsInPartnerHouses(this));

            await Task.WhenAll(sinastryAspect1Task, sinastryAspect2Task, sinastryStars1, sinastryStars2);

            return new SinastryBundle(this, match, sinastryAspect1Task.Result, sinastryAspect2Task.Result, sinastryStars1.Result, sinastryStars2.Result);
        }

        public void ClearHoroscope()
        {
            Horoscope?.RadixAspects.Clear();
            Horoscope = null;
        }

        public override string ToString() =>
        $"{SkyInfo}";

    }

    public class ReturnSkyEvent : AbstractSkyEvent
    {
        public House? HouseHostingReturnAsc { get; private set; }
        private SkyEvent _skyEvent;
        private Aspect _conj => Aspects.First(s => s.IsConjunction);

        public ReturnSkyEvent(DateTime date, TimeSpan time, City city, SkyEvent skyEvent, SkyType skyType) : base(date, time, city)
        {
            this._skyEvent = skyEvent;
            Person = this._skyEvent.Person;
            SkyInfo.SkyType = skyType;
            CalculateReturnAsc();
        }

        public void CalculateReturnAsc()
        {
            Houses[0].PointName = $"R. {Houses[0].PointName}";
            House returnAsc = Houses[0];
            HouseHostingReturnAsc = returnAsc.PlaceInHouse(_skyEvent);

            foreach (House radixHouse in _skyEvent.Houses)
            {
                double dist = PositionCalculator.CalculateDistance(returnAsc.EclipticLongitude, radixHouse.EclipticLongitude);
                Aspect? calculatedAspect = PositionCalculator.IsValidAspect(_conj, dist, 2.5);
                if (calculatedAspect == null) continue;
                calculatedAspect.TransitType = new(4);
                calculatedAspect.DateOf = SkyInfo.LocalDateTime;
                AddRadixAspect(calculatedAspect, returnAsc, radixHouse);
            }
        }

        protected override void CalculateBirthAspects()
        {
            RadixAspects.Clear();

            foreach (IStar star in Stars)
            {
                foreach (House pointReceiver in Houses)
                {

                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    if (distance > 2.5 || distance < -2.5)
                    {
                        continue;
                    }

                    Aspect? calculatedAspect = PositionCalculator.IsValidAspect(_conj, distance);

                    if (calculatedAspect != null)
                    {
                        calculatedAspect.TransitType = new(4);
                        calculatedAspect.DateOf = SkyInfo.LocalDateTime;
                        AddRadixAspect(calculatedAspect, star, pointReceiver);
                    }
                }
            }
        }

        private static bool IsEvilHouse(long id) =>
        id switch
        {
            1 or 6 or 12 => true,
            _ => false,
        };

        public bool WarnReturn(House inNatalHouse)
        {
            if (SkyInfo.SkyTypeId < 4) return false;

            Star mars = Stars[4];
            Star sun = Stars[0];

            if (IsEvilHouse(sun.House.PointId)) return true;

            if (IsEvilHouse(mars.House.PointId)) return true;

            if (IsEvilHouse(inNatalHouse.PointId)) return true;

            if (Stelliums != null)
                if (Stelliums.Any(s => IsEvilHouse(s.Point!.PointId)))
                    return true;

            foreach (Aspect aspect in RadixAspects)
            {
                House conjHouse;

                if (aspect.PointA is Star star && (star.PointId == 0 || star.PointId == 4))
                {
                    conjHouse = (House)aspect.PointB;
                    if (IsEvilHouse(conjHouse.PointId)) return true;
                }

                if (aspect.PointA is House house)
                {
                    conjHouse = (House)aspect.PointB;
                    if (IsEvilHouse(conjHouse.PointId)) return true;
                    if (IsEvilHouse(House.SlidHouse(aspect.OrbDiff, conjHouse))) return true;
                }
            }

            return false;
        }

    }

    public class SinastryBundle(SkyEvent sky1, SkyEvent sky2, IEnumerable<Aspect> aspects, IEnumerable<Aspect> aspects2, IEnumerable<Star>? stars, IEnumerable<Star>? stars2)
    {
        public SkyEvent Sky1 { get; } = sky1;
        public SkyEvent Sky2 { get; } = sky2;
        public Person Person1 => Sky1.Person!;
        public Person Person2 => Sky2.Person!;
        public IEnumerable<Aspect> Chart1Aspects { get; } = aspects;
        public IEnumerable<Aspect> Chart2Aspects { get; } = aspects2;
        public IEnumerable<Star>? Chart1Zodiac { get; } = stars;

        public string Chart1ZodiacTile => $"{Person1}'s Stars in {Person2}'s Houses";
        public IEnumerable<Star>? Chart2Zodiac { get; } = stars2;
        public string Chart2ZodiacTile => $"{Person2}'s Stars in {Person1}'s Houses";

        public string Title => $"{Person1} AND {Person2}";

        public IEnumerable<ElementGroupKey>? Chart1OccupiedHouses => Chart1Zodiac?.GroupBy(s => s.House)
                            .Select(s => new ElementGroupKey(s.Key, s.Count()))
                            .OrderByDescending(s => s.Count).ToList();

        public IEnumerable<ElementGroupKey>? Chart1Stelliums => Chart1OccupiedHouses?.Where(s => s.Count >= 3).ToList();

        public IEnumerable<ElementGroupKey>? Chart2OccupiedHouses => Chart2Zodiac?.GroupBy(s => s.House)
                    .Select(s => new ElementGroupKey(s.Key, s.Count()))
                    .OrderByDescending(s => s.Count).ToList();

        public IEnumerable<ElementGroupKey>? Chart2Stelliums => Chart2OccupiedHouses?.Where(s => s.Count >= 3).ToList();

    }
}