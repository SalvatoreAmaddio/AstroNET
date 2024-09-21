using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(Energy))]
    public class Energy : AbstractModel<Energy>
    {
        private Int64 _energyId;
        private string _energyName = string.Empty;

        [PK]
        public Int64 EnergyId { get => _energyId; set => UpdateProperty(ref value, ref _energyId); }
        [Field]
        public string EnergyName { get => _energyName; set => UpdateProperty(ref value, ref _energyName); }

        public Energy() { }

        public Energy(Int64 id) => _energyId = id;
        public Energy(DbDataReader reader) 
        {
            _energyId = reader.GetInt64(0);
            _energyName = reader.GetString(1);
        }

        public override string ToString()
        {
            return $"{EnergyName}";
        }
    }
}