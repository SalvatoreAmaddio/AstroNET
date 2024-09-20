using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(StarTransitOrbit))]
    public class StarTransitOrbit : AbstractModel<StarTransitOrbit>
    {
        private Int64 _starTransitId;
        private Star _star = null!;
        private Aspect _aspect = null!;
        private double _tollerance;

        [PK]
        public Int64 StarTransitId { get => _starTransitId; set => UpdateProperty(ref value, ref _starTransitId); }

        [Field]
        public Star Star { get => _star; set => UpdateProperty(ref value, ref _star); }

        [Field]
        public Aspect Aspect { get => _aspect; set => UpdateProperty(ref value, ref _aspect); }

        [Field]
        public double Tollerance { get => _tollerance; set => UpdateProperty(ref value, ref _tollerance); }
        public StarTransitOrbit() { }
        public StarTransitOrbit(DbDataReader reader) 
        {
            _starTransitId = reader.GetInt64(0);
            _star = new Star(reader.GetInt64(1));
            _aspect = new Aspect(reader.GetInt64(2));
            _tollerance = reader.GetDouble(3);
        }
    }
}