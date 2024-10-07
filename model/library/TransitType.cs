using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    public interface ITransitType
    {
        Int64 TransitTypeId { get; set; }
        string TransitTypeName { get; set; }
    }

    [Table(nameof(TransitType))]
    public class TransitType : AbstractModel<TransitType>, ITransitType
    {
        private Int64 _transitTypeId;
        private string _transitTypeName = string.Empty;

        [PK]
        public Int64 TransitTypeId { get => _transitTypeId; set => UpdateProperty(ref value, ref _transitTypeId); }
        [Field]
        public string TransitTypeName { get => _transitTypeName; set => UpdateProperty(ref value, ref _transitTypeName); }

        public TransitType() { }
        public TransitType(Int64 id) => _transitTypeId = id;
        public TransitType(DbDataReader reader)
        {
            _transitTypeId = reader.GetInt64(0);
            _transitTypeName = reader.GetString(1);
        }

        public override string ToString()
        {
            return $"{TransitTypeName}";
        }
    }
}
