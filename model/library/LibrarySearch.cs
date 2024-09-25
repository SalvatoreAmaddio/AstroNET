using Backend.Database;
using Backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.model
{
    public class LibrarySearch
    {
        private static IEnumerable<T>? GetLibrary<T>(TransitType transitType) where T : AbstractLibrary<T>, new() => 
        DatabaseManager.Find<T>()?.MasterSource.Cast<T>().Where(s => s.TransitType!.Equals(transitType)).ToList();
        
        public static List<ILibrary?> SearchAspect(Aspect aspect)
        {
            List<ILibrary?> Lib = [];

            IEnumerable<LibraryHouses>? houseLibrary = GetLibrary<LibraryHouses>(aspect.TransitType);
            IEnumerable<LibraryAspects>? aspectsLibrary = GetLibrary<LibraryAspects>(aspect.TransitType)?.Where(s => FindAspect(s, aspect)).ToList();

            if (aspectsLibrary != null) 
            {
                foreach (var item in aspectsLibrary)
                {
                    item.Aspect = aspect;
                    Lib.Add(item);
                }
            }

            Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, (Star)aspect.PointA, ((Star)aspect.PointA).House)));

            try
            {
                Lib.Add(houseLibrary?.FirstOrDefault(s => FindHouse(s, (Star)aspect.PointB, ((Star)aspect.PointB).House)));
            }
            catch
            {
                if (aspect.Orbit == 0) 
                {
                    LibraryHouses? prev = (LibraryHouses?)Lib[Lib.Count - 1];
                    prev = new LibraryHouses() { Aspect = aspect, Star = prev.Star, House = prev.House, Description = prev.Description };
                    Lib[Lib.Count - 1] = prev;

                    if (aspect.OrbDiff >= -2.5 && aspect.OrbDiff <= 2.5) 
                    {
                        House? house;
                        long id;

                        if (aspect.OrbDiff < 0)
                        {
                            id = (prev.House.PointId == 1) ? 12 : prev.House.PointId-1;
                        }
                        else 
                        {
                           id = (prev.House.PointId == 12) ? 1 : prev.House.PointId + 1;
                        }

                        house = new(id);
                        LibraryHouses? a = houseLibrary?.FirstOrDefault(s => FindHouse(s, (Star)aspect.PointA, house));
                        Lib.Add(a);
                    }
                }
                else 
                {
                    LibraryHouses? a = houseLibrary?.FirstOrDefault(s => FindHouse(s, (Star)aspect.PointA, (House)aspect.PointB));
                    if (a != null)
                        Lib.Add(new LibraryHouses() { Aspect = aspect, Star = a.Star, House = a.House, Description = a.Description });
                }
            }

            return Lib;
        }

        public static bool FindHouse(LibraryHouses library, Star? star, IHouse? house)
        {
            return library.Star.Equals(star) && library.House.Equals(house);
        }

        public static bool FindAspect(LibraryAspects library, Aspect aspect) 
        {
            if (aspect.PointB is House) return false;
            if (aspect.Energy.EnergyId == 3)
                return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB);
            return library.Star.Equals(aspect.PointA) && library.Star2.Equals(aspect.PointB) && library.Energy.Equals(aspect.Energy);
        }
    }
}
