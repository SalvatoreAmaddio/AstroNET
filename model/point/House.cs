using AstroNETLibrary.Points;
using Backend.Database;
using Backend.Model;
using System.Data.Common;
using System.Net;

namespace AstroNET.model
{
    [Table(nameof(House))]
    public class House : AbstractPoint<House>, IHouse
    {
        private bool _isAngular = false;
        private readonly Sign _rullerSign = null!;
        public Sign RullerSign { get => _rullerSign; }
        public bool IsAngular { get => _isAngular; set => UpdateProperty(ref value, ref _isAngular); }
        public double MidPoint { get; private set; }
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
                default:
                    break;
            }
        }

        public House(int houseId, double eclipticLongitude) : this(houseId)
        {
            EclipticLongitude = eclipticLongitude;
            RadixSign = new(GetZodiacSign(EclipticLongitude, out _, out _, out _));
            RadixSign = DatabaseManager.Find<Sign>()?.MasterSource.Cast<Sign>().First(s => s.SignId == RadixSign.SignId)!;
        }

        public static IHouse CreateHouse(int houseId, double eclipticLongitude)
        {
            return new House(houseId, eclipticLongitude);
        }

        public ISign GetRullerSig() => _rullerSign;

        public void CalculateMidPoint(double end)
        {
            double x1 = EclipticLongitude;

            // Normalize both angles to the range [0, 360)
            x1 = (x1 + 360) % 360;
            end = (end + 360) % 360;

            // Calculate direct angle difference
            double diff = Math.Abs(end - x1);

            if (diff <= 180)
            {
                // Directly average if within 180 degrees
                MidPoint = (x1 + end) / 2;
            }
            else
            {
                // Handle wrap-around case
                MidPoint = (x1 + end + 360) / 2;
                if (MidPoint >= 360)
                {
                    MidPoint -= 360;
                }
            }

            // Normalize midpoint to range [0, 360)
            MidPoint = (MidPoint + 360) % 360;
        }

    }
}