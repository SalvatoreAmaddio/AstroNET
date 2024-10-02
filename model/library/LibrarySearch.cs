using Backend.Database;

namespace AstroNET.model
{
    public class LibrarySearch
    {
        private static IEnumerable<T>? GetLibrary<T>(TransitType transitType) where T : AbstractStarLibrary<T>, new() =>
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType!.Equals(transitType)).ToList();

        private static IEnumerable<T>? GetLibrary<T>(Star star, int transitType = 1) where T : AbstractStarLibrary<T>, new() =>
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType.TransitTypeId == transitType && s.Star!.Equals(star)).ToList();

        private static IEnumerable<LibraryHouseSigns>? GetHouseSignLibrary(House house) =>
        DatabaseManager.Find<LibraryHouseSigns>()?.MasterSource.Cast<LibraryHouseSigns>().Where(s => s.House!.Equals(house)).ToList();

        public static List<IHouseLibrary?> SearchHouseSignDescription(House house)
        {
            List<IHouseLibrary?> lib = [];
            LibraryHouseSigns? record = GetHouseSignLibrary(house)?.FirstOrDefault(s => s.Sign.Equals(house.RadixSign));
            lib.Add(record);
            return lib;
        }

        public static List<IStarLibrary?> SearchStarDescription(Star star, int transitType = 1)
        {
            List<IStarLibrary?> lib = [];
            LibraryStarSigns? record = GetLibrary<LibraryStarSigns>(star, (transitType == 3) ? 1 : transitType)?
                .FirstOrDefault(s => s.Sign.Equals(star.RadixSign));

            lib.Add(record);

            StarInHouseDescription(ref lib, star, new(transitType));

            return lib;
        }

        public static List<IStarLibrary?> SearchAspect(Aspect aspect, House? houseHostingRAsc = null)
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

            if (aspect.PointAIsStar())
            {
                star1 = (Star)aspect.PointA;
                StarInHouseDescription(ref lib, star1, aspect.TransitType);
            }
            else
            {
                star1 = new(houseHostingRAsc);
            }

            if (aspect.PointBIsHouse())
            {
                House house = (House)aspect.PointB;

                if (aspect.IsConjunction)
                    StarOnCuspidHouse(ref lib, aspect, star1, aspect.TransitType);
                else
                    StarAspectHouseDescription(ref lib, aspect, star1, house, aspect.TransitType);
            }
            else
            {
                Star star2 = (Star)aspect.PointB;
                StarInHouseDescription(ref lib, star2, aspect.TransitType);
            }

            return lib;
        }

        public static void StarOnCuspidHouse(ref List<IStarLibrary?> Lib, Aspect aspect, Star star, TransitType transitType)
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);

            if (aspect.IsInCuspid)
            {
                long houseId = House.SlidHouse(aspect.OrbDiff, star.House);
                House cuspHouse = new(houseId);
                LibraryStarHouses? nextHouseRecord = houseLibrary?.FirstOrDefault(s => FindHouse(s, star, cuspHouse));
                Lib.Add(nextHouseRecord);
            }
        }

        private static void StarAspectHouseDescription(ref List<IStarLibrary?> Lib, Aspect aspect, Star star, IHouse house, TransitType transitType)
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);

            LibraryStarHouses? cuspidHouse = houseLibrary?.FirstOrDefault(s => FindHouse(s, star, house));

            if (cuspidHouse == null) return;

            LibraryStarHouses record = new(aspect, cuspidHouse.Star, cuspidHouse.House, cuspidHouse.Description);
            Lib.Add(record);
        }

        private static void StarInHouseDescription(ref List<IStarLibrary?> Lib, Star star, TransitType transitType)
        {
            IEnumerable<LibraryStarHouses>? houseLibrary = GetLibrary<LibraryStarHouses>(transitType);
            LibraryStarHouses? record = houseLibrary?.FirstOrDefault(s => FindHouse(s, star, star.House));
            Lib.Add(record);
        }

        public static bool FindHouse(LibraryStarHouses library, Star? star, IHouse? house) =>
        library.Star.Equals(star) && library.House.Equals(house);

        public static bool FindAspect(LibraryStarAspects library, Aspect aspect)
        {
            if (aspect.PointB is House) return false;
            if (aspect.Energy.EnergyId == 3)
                return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB);
            return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB) && library.Energy.Equals(aspect.Energy);
        }
    }
}