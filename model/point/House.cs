using Backend.Database;
using Backend.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(House))]
    public class House : AbstractPoint<House>, IHouse
    {
        private bool _isAngular = false;
        private readonly Sign _rullerSign = null!;
        public Sign RullerSign { get => _rullerSign; }
        public bool IsAngular { get => _isAngular; set => UpdateProperty(ref value, ref _isAngular); }
        public House() { }
        public House(Int64 id) : base(id)
        {
            _rullerSign = new Sign(id);
            SetAngulars();
        }

        public House(DbDataReader reader) : base(reader)
        {
            _rullerSign = new Sign(_pointId);
            SetAngulars();
        }

        private void SetAngulars() 
        {
            switch (PointId)
            {
                case 1:
                case 4:
                case 7:
                case 10:
                    _isAngular = true;
                    break;
            }
        }

        public House(int houseId, double eclipticLongitude) : this(houseId)
        {
            EclipticLongitude = eclipticLongitude;
            RadixSign = new(GetZodiacSign(EclipticLongitude, out _, out _, out _));
            RadixSign = DatabaseManager.Find<Sign>()?.MasterSource.Cast<Sign>().First(s => s.SignId == RadixSign.SignId)!;
        }

        public static long SlidHouse(double orbDiff, IHouse house)
        {
            if (orbDiff < 0) return (house.PointId == 1) ? 12 : house.PointId - 1;
            return (house.PointId == 12) ? 1 : house.PointId + 1;
        }
    }
}