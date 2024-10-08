using AstroNETLibrary.Points;
using AstroNETLibrary.Sky;
using Backend.Database;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    public abstract class AbstractPoint<M> : AbstractModel<M>, IPoint where M : ISQLModel, IPoint, new()
    {
        protected Int64 _pointId;
        protected string _pointName = string.Empty;
        protected string _description = string.Empty;
        protected double _eclipticLongitude;
        protected Position _position = null!;
        protected Sign _radixSign = null!;
        protected IPoint? _fetchDbPoint;

        [PK]
        public Int64 PointId { get => _pointId; set => UpdateProperty(ref value, ref _pointId); }

        [Field]
        public string PointName { get => _pointName; set => UpdateProperty(ref value, ref _pointName); }

        [Field]
        public string Description { get => _description; set => UpdateProperty(ref value, ref _description); }
        public Sign RadixSign { get => _radixSign; protected set => UpdateProperty(ref value, ref _radixSign); }

        public double EclipticLongitude { get => _eclipticLongitude; protected set => UpdateProperty(ref value, ref _eclipticLongitude); }
        public Position Position { get => _position; protected set => UpdateProperty(ref value, ref _position); }
        public AbstractPoint() { }
        public AbstractPoint(Int64 id) 
        {
            _pointId = id;
        }

        public virtual void Build() 
        {
            _fetchDbPoint = DatabaseManager.Find<M>()?.MasterSource.Cast<IPoint>().First(s => s.PointId == _pointId);
            if (_fetchDbPoint == null) return;
            _radixSign?.Build();
            _pointName = _fetchDbPoint.PointName;
            _description = _fetchDbPoint.Description;
        }

        public AbstractPoint(DbDataReader reader)
        {
            _pointId = reader.GetInt64(0);
            _pointName = reader.GetString(1);
            _description = reader.GetString(2);
        }

        public override string ToString()
        {
            return $"{PointName}";
        }

        public virtual IHouse PlaceInHouse(AbstractSkyEvent sky)
        {
            for (int i = 0; i < 12; i++)
            {
                int nextHouseIndex = i == 11 ? 1 : i + 1;

                if (sky.Houses[i].EclipticLongitude <= sky.Houses[nextHouseIndex].EclipticLongitude)
                {
                    if (EclipticLongitude >= sky.Houses[i].EclipticLongitude && EclipticLongitude < sky.Houses[nextHouseIndex].EclipticLongitude)
                    {
                        return sky.Houses[i];
                    }
                }
                else
                {
                    if (EclipticLongitude >= sky.Houses[i].EclipticLongitude || EclipticLongitude < sky.Houses[nextHouseIndex].EclipticLongitude)
                    {
                        return sky.Houses[i];
                    }
                }
            }
            throw new Exception();
        }

        protected int GetZodiacSign(double longitude, out double degreesInSign, out int minutesInSign, out int secondsInSign)
        {
            int signIndex = (int)(longitude / 30.0);
            degreesInSign = longitude % 30; // Remainder gives degrees within the sign
            minutesInSign = (int)((degreesInSign - Math.Floor(degreesInSign)) * 60); // Convert the fractional part to minutes
            double fractionalMinutes = (degreesInSign - Math.Floor(degreesInSign)) * 60;
            secondsInSign = (int)((fractionalMinutes - minutesInSign) * 60); // Convert the fractional part of minutes to seconds
            degreesInSign = Math.Floor(degreesInSign); // Only take the integer part for degrees

            Position = new(degreesInSign, minutesInSign, secondsInSign);
            return signIndex + 1;
        }

        public ISign GetRadixSign() => _radixSign;
    }
}