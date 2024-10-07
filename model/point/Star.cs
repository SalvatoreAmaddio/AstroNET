using Backend.Database;
using Backend.Model;
using System.Data.Common;

namespace AstroNET.model
{

    [Table(nameof(Star))]

    public class Star : AbstractPoint<Star>, IStar
    {
        private int _cycleLength;
        private bool _isRetrograde = false;
        private string _uri = string.Empty;
        private IHouse _radixHouse = null!;
        private double _transitOrbit;
        private Energy _energy = null!;
        private Element _element = null!;
        private string _inHouseDescription = string.Empty;
        private string _transitDescription = string.Empty;
        private string _returnDescription = string.Empty;

        [Field]
        public int CycleLength { get => _cycleLength; set => UpdateProperty(ref value, ref _cycleLength); }

        [Field]
        public string URI { get => _uri; set => UpdateProperty(ref value, ref _uri); }

        [Field]
        public double TransitOrbit { get => _transitOrbit; protected set => UpdateProperty(ref value, ref _transitOrbit); }

        [FK]
        public Energy Energy { get => _energy; private set => UpdateProperty(ref value, ref _energy); }

        [FK]
        public Element Element { get => _element; private set => UpdateProperty(ref value, ref _element); }

        [Field]
        public string InHouseDescription { get => _inHouseDescription; set => UpdateProperty(ref value, ref _inHouseDescription); }

        [Field]
        public string TransitDescription { get => _transitDescription; set => UpdateProperty(ref value, ref _transitDescription); }

        [Field]
        public string ReturnDescription { get => _returnDescription; set => UpdateProperty(ref value, ref _returnDescription); }

        public bool IsRetrograde { get => _isRetrograde; private set => UpdateProperty(ref value, ref _isRetrograde); }
        public IHouse House { get => _radixHouse; private set => UpdateProperty(ref value, ref _radixHouse); }
        public Star() { }

        public Star(DbDataReader reader) : base(reader)
        {
            _uri = reader.GetString(3);
            _cycleLength = reader.GetInt32(4);
            _transitOrbit = reader.GetDouble(5);
            _energy = new(reader.GetInt32(6));
            _element = new(reader.GetInt64(7));
            _inHouseDescription = reader.GetString(8);
            _transitDescription = reader.GetString(9);
            _returnDescription = reader.GetString(10);
        }

        public Star(Int64 id) : base(id)
        {
        }

        public Star(IHouse house)
        {
            _radixHouse = house;
        }

        public Star(int ID, ref double[] xx) : this(ID)
        {
            EclipticLongitude = xx[0];
            IsRetrograde = xx[3] < 0;
            RadixSign = DatabaseManager.Find<Sign>()?.MasterSource.Cast<Sign>().First(s => s.SignId == GetZodiacSign(EclipticLongitude, out _, out _, out _))!;
        }
        public Star(int ID, ref double[] xx, ref double[] cusps) : this(ID, ref xx)
        {
            (int id, double eclipticLongitude) = FindHouse(cusps);
            House = new House(id, eclipticLongitude);
        }

        protected (int houseIndex, double cuspStart) FindHouse(double[] cusps)
        {
            for (int i = 1; i <= 12; i++)
            {
                int nextHouseIndex = i == 12 ? 1 : i + 1;

                if (cusps[i] <= cusps[nextHouseIndex])
                {
                    if (EclipticLongitude >= cusps[i] && EclipticLongitude < cusps[nextHouseIndex])
                    {
                        return (i, cusps[i]);
                    }
                }
                else
                {
                    if (EclipticLongitude >= cusps[i] || EclipticLongitude < cusps[nextHouseIndex])
                    {
                        return (i, cusps[i]);
                    }
                }
            }
            throw new Exception("HouseIndex not found!");
        }

        public override void Build()
        {
            base.Build();
            Star star = (Star)_fetchDbPoint!;
            _uri = star.URI;
            _transitOrbit = star.TransitOrbit;
            _element = star.Element;
            House?.Build();
        }

        public override IHouse PlaceInHouse(AbstractSkyEvent sky)
        {
            for (int i = 0; i < 12; i++)
            {
                int nextHouseIndex = i == 11 ? 1 : i + 1;

                if (sky.Houses[i].EclipticLongitude <= sky.Houses[nextHouseIndex].EclipticLongitude)
                {
                    if (EclipticLongitude >= sky.Houses[i].EclipticLongitude && EclipticLongitude < sky.Houses[nextHouseIndex].EclipticLongitude)
                    {
                        House = sky.Houses[i];
                        return sky.Houses[i];
                    }
                }
                else
                {
                    if (EclipticLongitude >= sky.Houses[i].EclipticLongitude || EclipticLongitude < sky.Houses[nextHouseIndex].EclipticLongitude)
                    {
                        House = sky.Houses[i];
                        return sky.Houses[i];
                    }
                }
            }

            throw new Exception();
        }

        public override bool IsNewRecord() => (long?)GetPrimaryKey()?.GetValue() == -1;

        public IAstrologyAttribute GetElement() => _element;

    }
}