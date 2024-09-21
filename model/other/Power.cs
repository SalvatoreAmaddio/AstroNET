using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(Power))]
    public class Power : AbstractModel<Power>
    {
        private Int64 _powerId;
        private string _powerName = string.Empty;

        [PK]
        public Int64 PowerID { get => _powerId; set => UpdateProperty(ref value, ref  _powerId); }
        [Field]
        public string PowerName { get => _powerName; set => UpdateProperty(ref value, ref _powerName); }

        public Power() { }
        public Power(Int64 id) => _powerId = id;
        public Power(DbDataReader reader) 
        {
            _powerId = reader.GetInt64(0);
            _powerName = reader.GetString(1);
        }

        public override string ToString() => PowerName;
    }
}
