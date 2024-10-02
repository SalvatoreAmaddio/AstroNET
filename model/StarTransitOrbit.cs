using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    [Table(nameof(StarTransitOrbit))]
    public class StarTransitOrbit : AbstractModel<StarTransitOrbit>
    {
        private Int64 _starTransitOrbitId;
        private Star _star = null!;
        private Aspect _aspect = null!;
        private double _tollerance;

        [PK]
        public Int64 StarTransitOrbitId { get => _starTransitOrbitId; set => UpdateProperty(ref value, ref _starTransitOrbitId); }

        [FK]
        public Star Star { get => _star; set => UpdateProperty(ref value, ref _star); }

        [FK]
        public Aspect Aspect { get => _aspect; set => UpdateProperty(ref value, ref _aspect); }

        [Field]
        public double Tollerance { get => _tollerance; set => UpdateProperty(ref value, ref _tollerance); }
        public StarTransitOrbit() { }
        public StarTransitOrbit(DbDataReader reader) 
        {
            _starTransitOrbitId = reader.GetInt64(0);
            _star = new Star(reader.GetInt64(1));
            _aspect = new Aspect(reader.GetInt64(2));
            _tollerance = reader.GetDouble(3);
        }
    }
}