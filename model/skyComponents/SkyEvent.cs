using MvvmHelpers;
using System.ComponentModel;

namespace AstroNET.model
{
    public abstract class AbstractSkyEvent
    {
        #region Properties
        public SkyInfo SkyInfo { get; private set; } = new();
        public ObservableRangeCollection<IStar> Stars { get; private set; } = [];
        public IEnumerable<ElementGroupKey>? OccupiedHouses =>
        (Houses.Count == 0) ? [] : Stars.GroupBy(s => s.House)
                                        .Select(s => new ElementGroupKey(s.Key, s.Count()))
                                        .OrderByDescending(s => s.Count).ToList();

        public IEnumerable<ElementGroupKey>? Stelliums => OccupiedHouses?.Where(s => s.Count >= 3).ToList();
        public ObservableRangeCollection<IHouse> Houses { get; protected set; } = [];
        public INotifyPropertyChanged? PersonController { get; protected set; }
        public IPerson Person { get; protected set; } = null!;
        public int FireSigns => Stars.Count(s => s.GetRadixSign().GetElement().ID() == 1);
        public int WaterSigns => Stars.Count(s => s.GetRadixSign().GetElement().ID() == 2);
        public int AirSigns => Stars.Count(s => s.GetRadixSign().GetElement().ID() == 3);
        public int EarhSigns => Stars.Count(s => s.GetRadixSign().GetElement().ID() == 4);
        public int MaleSigns => Stars.Count(s => s.GetRadixSign().GetGender().ID() == 1);
        public int FemaleSigns => Stars.Count(s => s.GetRadixSign().GetGender().ID() == 2);
        public int CardinalSigns => Stars.Count(s => s.GetRadixSign().GetTriplicity().ID() == 3);
        public int FixedSigns => Stars.Count(s => s.GetRadixSign().GetTriplicity().ID() == 1);
        public int MobileSigns => Stars.Count(s => s.GetRadixSign().GetTriplicity().ID() == 2);

        public List<IPoint> Aspectables { get; } = [];
        public ObservableRangeCollection<IAspect> RadixAspects { get; } = [];

        public static Func<IEnumerable<IStarTransitOrbit>>? FetchTransitAspects;
        public static Func<List<IAspect>>? FetchAspects;
        protected List<IAspect> Aspects
        {
            get
            {
                if (FetchAspects == null) throw new NotImplementedException();
                return FetchAspects();
            }
        }

        #endregion

        public AbstractSkyEvent() { }
        public AbstractSkyEvent(DateTime date, TimeSpan time, ICity city)
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

                    foreach (IAspect aspect in Aspects)
                    {
                        if (pointReceiver is IHouse house && !house.IsAngular && aspect.Orbit != 0)
                            continue;

                        IAspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance);

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

        protected void AddRadixAspect(IAspect? aspect, IPoint pointA, IPoint pointB)
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

        public IEnumerable<IAspect> CalculateStarAspects(IStar star, DateTime date)
        {
            if (FetchTransitAspects == null) throw new NotImplementedException("");
            IEnumerable<IStarTransitOrbit>? transitAspects = FetchTransitAspects().ToList();

            if (Person != null && !Person.UnknownTime)
                star.PlaceInHouse(this);

            foreach (IPoint pointReceiver in this.Aspectables)
            {
                double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                foreach (IAspect aspect in Aspects)
                {
                    double? tollerance = transitAspects.FirstOrDefault(s => s.GetAspect().Equals(aspect) && s.GetStar().Equals(star))?.Tollerance;
                    if (tollerance == null) continue;

                    if (pointReceiver is IHouse house && !house.IsAngular && !aspect.IsConjunction)
                        continue;

                    IAspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                    if (calculatedAspect != null)
                    {
                        if (pointReceiver is IHouse house2 && !house2.IsAngular)
                        {
                            if (distance > 2.5 || distance < -2.5)
                                continue;
                        }

                        calculatedAspect.SetTransitType(2);

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

        public SkyEvent(IPerson person, INotifyPropertyChanged controller) : this(person)
        {
            PersonController = controller;
        }

        public SkyEvent(IPerson person) : this(person.DOB, person.TOB, person.GetCity(), !person.UnknownTime)
        {
            Person = person;
        }

        public SkyEvent(IPerson person, bool showHouses)
            : this(person.DOB, person.TOB, person.GetCity(), showHouses) =>
            Person = person;

        public SkyEvent(DateTime date, TimeSpan time, ICity city, bool showHouses = true) : base(date, time, city)
        {
            ShowHouses = showHouses;
            CalculatePositions();
        }

        public SkyEvent(int year, int month, int day, int hour, int minutes, ICity city, bool showHouses = true) : this()
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

        public ReturnSkyEvent CalculateReturn(DateTime returnDate, TimeSpan returnTime, ICity selectedCity, SkyType skyType = SkyType.SunReturn) =>
        new(returnDate, returnTime, selectedCity, this, skyType);

        public SkyEvent CloneMe() =>
        new(SkyInfo.Year, SkyInfo.Month, SkyInfo.Day, SkyInfo.LocalTime.Hours, SkyInfo.LocalTime.Minutes, SkyInfo.City, ShowHouses) { Person = this.Person };

        private static void CompleteAspect(ref IAspect aspect, IPoint pointA, IPoint pointB)
        {
            aspect.PointA = pointA;

            pointB.Build();

            aspect.PointB = pointB;

            aspect.CalculateOrbDiff();
        }

        public void CalculateHoroscope(DateTime date, TimeSpan time, ICity city)
        {
            if (FetchTransitAspects == null) throw new NotImplementedException("");
            IEnumerable<IStarTransitOrbit>? transitAspects = FetchTransitAspects().ToList();

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

                    foreach (IAspect aspect in Aspects)
                    {
                        double? tollerance = transitAspects.FirstOrDefault(s => s.GetAspect().Equals(aspect) && s.GetStar().Equals(star))?.Tollerance;

                        if (tollerance == null) continue;

                        if (pointReceiver is IHouse house && !house.IsAngular && !aspect.IsConjunction)
                            continue;

                        IAspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                        if (calculatedAspect != null)
                        {
                            if (pointReceiver is IHouse house2 && !house2.IsAngular)
                            {
                                if (distance > 2.5 || distance < -2.5)
                                    continue;
                            }

                            calculatedAspect.SetTransitType(2);
                            calculatedAspect.DateOf = date;
                            Horoscope.AddRadixAspect(calculatedAspect, star, pointReceiver);
                        }
                    }
                }
            }
        }

        public IEnumerable<IStar>? StarsInPartnerHouses(SkyEvent match)
        {
            if (match.Person.UnknownTime) return null;
            PositionCalculator positionCalculator = new(this);

            List<IStar> stars = [];

            for (int i = 0; i < 10; i++)
            {
                stars.Add(positionCalculator.CalculatePlanet(i, !ShowHouses));
                stars[i].Build();
                stars[i].PlaceInHouse(match);
            }

            return stars;
        }

        public IEnumerable<IAspect> CalculateSinastry(SkyEvent match)
        {
            List<IAspect> aspects = [];
            if (FetchTransitAspects == null) throw new NotImplementedException("");
            IEnumerable<IStarTransitOrbit>? transitAspects = FetchTransitAspects().ToList();

            foreach (IStar star in Stars)
            {
                foreach (IPoint pointReceiver in match.Aspectables)
                {
                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    foreach (IAspect aspect in Aspects)
                    {
                        double? tollerance = transitAspects.FirstOrDefault(s => s.GetAspect().Equals(aspect) && s.GetStar().Equals(star))?.Tollerance;

                        if (tollerance == null) continue;

                        if (pointReceiver is IHouse house && !house.IsAngular && !aspect.IsConjunction)
                            continue;

                        IAspect? calculatedAspect = PositionCalculator.IsValidAspect(aspect, distance, tollerance.Value);

                        if (calculatedAspect != null)
                        {
                            if (pointReceiver is IHouse house2 && !house2.IsAngular)
                            {
                                if (distance > 2.5 || distance < -2.5)
                                    continue;
                            }

                            calculatedAspect.SetTransitType(3);
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
            Task<IEnumerable<IAspect>> sinastryAspect1Task = Task.Run(() => CalculateSinastry(match));
            Task<IEnumerable<IAspect>> sinastryAspect2Task = Task.Run(() => match.CalculateSinastry(this));
            Task<IEnumerable<IStar>?> sinastryStars1 = Task.Run(() => StarsInPartnerHouses(match));
            Task<IEnumerable<IStar>?> sinastryStars2 = Task.Run(() => match.StarsInPartnerHouses(this));

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
        public IHouse? HouseHostingReturnAsc { get; private set; }
        private SkyEvent _skyEvent;
        private IAspect _conj => Aspects.First(s => s.IsConjunction);

        public ReturnSkyEvent(DateTime date, TimeSpan time, ICity city, SkyEvent skyEvent, SkyType skyType) : base(date, time, city)
        {
            this._skyEvent = skyEvent;
            Person = this._skyEvent.Person;
            SkyInfo.SkyType = skyType;
            CalculateReturnAsc();
        }

        public void CalculateReturnAsc()
        {
            Houses[0].PointName = $"R. {Houses[0].PointName}";
            IHouse returnAsc = Houses[0];
            HouseHostingReturnAsc = returnAsc.PlaceInHouse(_skyEvent);

            foreach (IHouse radixHouse in _skyEvent.Houses)
            {
                double dist = PositionCalculator.CalculateDistance(returnAsc.EclipticLongitude, radixHouse.EclipticLongitude);
                IAspect? calculatedAspect = PositionCalculator.IsValidAspect(_conj, dist, 2.5);
                if (calculatedAspect == null) continue;
                calculatedAspect.SetTransitType(4);
                calculatedAspect.DateOf = SkyInfo.LocalDateTime;
                AddRadixAspect(calculatedAspect, returnAsc, radixHouse);
            }
        }

        protected override void CalculateBirthAspects()
        {
            RadixAspects.Clear();

            foreach (IStar star in Stars)
            {
                foreach (IHouse pointReceiver in Houses)
                {

                    double distance = PositionCalculator.CalculateDistance(star.EclipticLongitude, pointReceiver.EclipticLongitude);

                    if (distance > 2.5 || distance < -2.5)
                    {
                        continue;
                    }

                    IAspect? calculatedAspect = PositionCalculator.IsValidAspect(_conj, distance);

                    if (calculatedAspect != null)
                    {
                        calculatedAspect.SetTransitType(4);
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

        public bool WarnReturn(IHouse inNatalHouse)
        {
            if (SkyInfo.SkyTypeId < 4) return false;

            IStar mars = Stars[4];
            IStar sun = Stars[0];

            if (IsEvilHouse(sun.House.PointId)) return true;

            if (IsEvilHouse(mars.House.PointId)) return true;

            if (IsEvilHouse(inNatalHouse.PointId)) return true;

            if (Stelliums != null)
                if (Stelliums.Any(s => IsEvilHouse(s.Point!.PointId)))
                    return true;

            foreach (IAspect aspect in RadixAspects)
            {
                IHouse conjHouse;

                if (aspect.PointA is IStar star && (star.PointId == 0 || star.PointId == 4))
                {
                    conjHouse = (IHouse)aspect.PointB;
                    if (IsEvilHouse(conjHouse.PointId)) return true;
                }

                if (aspect.PointA is IHouse house)
                {
                    conjHouse = (IHouse)aspect.PointB;
                    if (IsEvilHouse(conjHouse.PointId)) return true;
                    if (IsEvilHouse(IHouse.SlidHouse(aspect.OrbDiff, conjHouse))) return true;
                }
            }

            return false;
        }

    }

    public class SinastryBundle(SkyEvent sky1, SkyEvent sky2, IEnumerable<IAspect> aspects, IEnumerable<IAspect> aspects2, IEnumerable<IStar>? stars, IEnumerable<IStar>? stars2)
    {
        public SkyEvent Sky1 { get; } = sky1;
        public SkyEvent Sky2 { get; } = sky2;
        public IPerson Person1 => Sky1.Person!;
        public IPerson Person2 => Sky2.Person!;
        public IEnumerable<IAspect> Chart1Aspects { get; } = aspects;
        public IEnumerable<IAspect> Chart2Aspects { get; } = aspects2;
        public IEnumerable<IStar>? Chart1Zodiac { get; } = stars;
        public string Chart1ZodiacTile => $"{Person1}'s Stars in {Person2}'s Houses";
        public IEnumerable<IStar>? Chart2Zodiac { get; } = stars2;
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