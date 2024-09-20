using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(Triplicity))]
    public class Triplicity : AbstractModel<Triplicity>
    {
        private Int64 _triplicityId;
        private string _triplicityName = string.Empty;

        [PK]
        public Int64 TriplicityId { get => _triplicityId; set => UpdateProperty(ref value, ref _triplicityId); }
        [Field]
        public string TriplicityName { get => _triplicityName; set => UpdateProperty(ref value, ref _triplicityName); }

        public Triplicity()
        {

        }

        public Triplicity(Int64 id)
        {
            _triplicityId = id;
        }

        public Triplicity(DbDataReader reader)
        {
            _triplicityId = reader.GetInt64(0);
            _triplicityName = reader.GetString(1);
        }

        public override string ToString() => TriplicityName;        
    }
}