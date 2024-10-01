using Backend.Database;

namespace WpfApp1.model
{
    public class LibrarySearch
    {
        private static IEnumerable<T>? GetLibrary<T>(TransitType transitType) where T : AbstractStarLibrary<T>, new() => 
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType!.Equals(transitType)).ToList();

        private static IEnumerable<T>? GetLibrary<T>(Star star, int transitType = 1) where T : AbstractStarLibrary<T>, new() =>
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType.TransitTypeId == transitType && s.Star!.Equals(star)).ToList();

        private static IEnumerable<LibraryHouseSigns>? GetHouseSignLibrary(House house) =>
        DatabaseManager.Find<LibraryHouseSigns>()?.MasterSource.Cast<LibraryHouseSigns>().Where(s => s.House!.Equals(house)).ToList();

        public static List<IHouseLibrary?> SearchHouseSign(House house) 
        {
            List<IHouseLibrary?> lib = [];
            long id = house.PointId;
            lib.Add(GetHouseSignLibrary(house)?.FirstOrDefault(s => s.Sign.Equals(house.RadixSign)));
            return lib;
        }

        public static List<IStarLibrary?> SearchStar(Star star, int transitType = 1) 
        {
            List<IStarLibrary?> lib = [];
            if (transitType == 3)
                lib.Add(GetLibrary<LibraryStarSigns>(star, 1)?.FirstOrDefault(s => s.Sign.Equals(star.RadixSign)));
            else lib.Add(GetLibrary<LibraryStarSigns>(star, transitType)?.FirstOrDefault(s => s.Sign.Equals(star.RadixSign)));

            StarInHouse(ref lib, star, new(transitType));
            return lib;
        }

        public static List<IStarLibrary?> SearchAspect(Aspect aspect)
        {
            List<IStarLibrary?> lib = [];

            IEnumerable<LibraryStarAspects>? aspectsLibrary = GetLibrary<LibraryStarAspects>(aspect.TransitType)?.Where(s => FindAspect(s, aspect)).ToList();

            if (aspectsLibrary != null) 
            {
                foreach (var item in aspectsLibrary)
                {
                    item.Aspect = aspect;
                    lib.Add(item);
                }
            }

            Star star1;

            try
            {
                star1 = (Star)aspect.PointA;
                StarInHouse(ref lib, star1, aspect.TransitType);
            }
            catch
            {
                star1 = new((House)aspect.PointB);
            }

            TransitType transitType;

            if (aspect.TransitType.TransitTypeId  == 4 || aspect.TransitType.TransitTypeId ==5) 
            {
                transitType = new(4);
            }
            else
            {
                transitType = new(1);
            }

            if (aspect.PointB is House house)
            {
                StarAspectHouse(ref lib, aspect, star1, house, transitType);
                StarCuspidHouse(ref lib, aspect, star1, transitType);
            }
            else
            {
                Star star2 = (Star)aspect.PointB;
                StarInHouse(ref lib, star2, transitType);
            }
            return lib;
        }

        public static void StarCuspidHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star, TransitType transitType)
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);

            if (aspect.Orbit == 0)
            {
                if (transitType.TransitTypeId == 4) 
                {
                    if (aspect.OrbDiff >= -2.5 && aspect.OrbDiff <= 2.5)
                    {
                        House cuspHouse = new(House.SlidHouse(aspect.OrbDiff, star.House));
                        Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, star, cuspHouse)));
                        return;
                    }
                }

                LibraryStarHouses? prev = (LibraryStarHouses?)Lib[Lib.Count - 1];
                if (prev == null) return;
                prev = new LibraryStarHouses { Aspect = aspect, Star = prev.Star, House = prev.House, Description = prev.Description };
                Lib[Lib.Count - 1] = prev;

                if (aspect.OrbDiff >= -2.5 && aspect.OrbDiff <= 2.5)
                {
                    House cuspHouse = new(House.SlidHouse(aspect.OrbDiff, prev.House));
                    Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, star, cuspHouse)));
                }
            }
        }

        private static void StarAspectHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star, IHouse house, TransitType transitType) 
        {
            if (aspect.Orbit == 0)
            {
                return;
            }

            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);
            LibraryStarHouses? cuspidHouse = houseLibrary?.FirstOrDefault(s => FindHouse(s, star, house));
            if (cuspidHouse == null) return;
            Lib.Add(new LibraryStarHouses() { Aspect = aspect, Star = cuspidHouse.Star, House = cuspidHouse.House, Description = cuspidHouse.Description});
        }

        private static void StarInHouse(ref List<IStarLibrary?> Lib, Star star, TransitType transitType) 
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);
            Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, star, star.House)));
        }

        public static bool FindHouse(LibraryStarHouses library, Star? star, IHouse? house)
        {
            return library.Star.Equals(star) && library.House.Equals(house);
        }

        public static bool FindAspect(LibraryStarAspects library, Aspect aspect) 
        {
            if (aspect.PointB is House) return false;
            if (aspect.Energy.EnergyId == 3)
                return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB);
            return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB) && library.Energy.Equals(aspect.Energy);
        }
    }
}