using Backend.Model;
using System.Data.Common;

namespace AstroNET.model
{
    [Table(nameof(StarPower))]
    public class StarPower : AbstractPoint<StarPower>
    {
        private Int64 _starPowerId;
        private Star? _star;
        private Power? _power;
        private Sign? _sign;

        [PK]
        public Int64 StarPowerID { get => _starPowerId; set => UpdateProperty(ref value, ref _starPowerId); }
        [FK]
        public Star? Star { get => _star; set => UpdateProperty(ref value, ref _star); }
        [FK]
        public Power? Power { get => _power; set => UpdateProperty(ref value, ref _power); }
        [FK]
        public Sign? Sign { get => _sign; set => UpdateProperty(ref value, ref _sign); }
        
        public StarPower() { }

        public StarPower(DbDataReader reader) 
        {
            _starPowerId = reader.GetInt64(0);
            _star = new(reader.GetInt64(1));
            _power = new(reader.GetInt64(2));
            _sign = new(reader.GetInt64(3));
        }
    }
}