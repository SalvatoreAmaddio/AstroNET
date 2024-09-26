using Backend.Database;

namespace WpfApp1.model
{
    public class LibrarySearch
    {
        private static IEnumerable<T>? GetLibrary<T>(TransitType transitType) where T : AbstractStarLibrary<T>, new() => 
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType!.Equals(transitType)).ToList();

        private static IEnumerable<T>? GetLibrary<T>(Star star) where T : AbstractStarLibrary<T>, new() =>
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.Star!.Equals(star)).ToList();

        private static IEnumerable<LibraryHouseSigns>? GetHouseSignLibrary(House house) =>
        DatabaseManager.Find<LibraryHouseSigns>()?.MasterSource.Cast<LibraryHouseSigns>().Where(s => s.House!.Equals(house)).ToList();

        private static long SlidHouse(double orbDiff, IHouse house) 
        {
            if (orbDiff < 0) return (house.PointId == 1) ? 12 : house.PointId - 1;
            return (house.PointId == 12) ? 1 : house.PointId + 1;
        }

        public static List<IHouseLibrary?> SearchHouse(House house) 
        {
            List<IHouseLibrary?> lib = [];
            lib.Add(GetHouseSignLibrary(house)?.FirstOrDefault(s => s.Sign.Equals(house.RadixSign)));
            return lib;
        }

        public static List<IStarLibrary?> SearchStar(Star star) 
        {
            List<IStarLibrary?> lib = [];
            lib.Add(GetLibrary<LibraryStarSigns>(star)?.FirstOrDefault(s => s.Sign.Equals(star.RadixSign)));
            StarInHouse(ref lib, new() { TransitType = new(1)}, star);
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

            Star star1 = (Star)aspect.PointA;
            StarInHouse(ref lib, aspect, star1);

            if (aspect.PointB is House house) 
            {
                StarAspectHouse(ref lib, aspect, star1, house);
                StarCuspidHouse(ref lib, aspect, star1);
            }
            else 
            {
                Star star2 = (Star)aspect.PointB;
                StarInHouse(ref lib, aspect, star2);
            }

            return lib;
        }
        
        public static void StarCuspidHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star) 
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(aspect.TransitType);

            if (aspect.Orbit == 0)
            {
                LibraryStarHouses? prev = (LibraryStarHouses?)Lib[Lib.Count - 1];
                prev = new LibraryStarHouses { Aspect = aspect, Star = prev.Star, House = prev.House, Description = prev.Description };
                Lib[Lib.Count - 1] = prev;

                if (aspect.OrbDiff >= -2.5 && aspect.OrbDiff <= 2.5)
                {
                    House cuspHouse = new(SlidHouse(aspect.OrbDiff, prev.House));
                    Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, star, cuspHouse)));
                }
            }
        }

        private static void StarAspectHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star, IHouse house) 
        {
            if (aspect.Orbit == 0)
            {
                    return;
            }
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(aspect.TransitType);
            LibraryStarHouses? cuspidHouse = houseLibrary?.FirstOrDefault(s => FindHouse(s, star, house));
            if (cuspidHouse == null) return;
            Lib.Add(new LibraryStarHouses() { Aspect = aspect, Star = cuspidHouse.Star, House = cuspidHouse.House, Description = cuspidHouse.Description});
        }

        private static void StarInHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star) 
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(aspect.TransitType);
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