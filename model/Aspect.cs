using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;
using System.Windows.Media;

namespace WpfApp1.model
{
    [Table(nameof(Aspect))]
    public class Aspect : AbstractModel<Aspect>
    {
        private Int64 _aspectId;
        private string _aspectName = string.Empty;
        private string _description = string.Empty;
        private bool _isMajor = false;
        private double _orbit;
        private double _natalTollerance;
        private bool _isDashed = false;
        private double _diff = 0;
        private string _colorHex = string.Empty;
        private string _uri = string.Empty;
        private Energy _energy = null!;
        
        [PK]
        public Int64 AspectId { get => _aspectId; set => UpdateProperty(ref value, ref _aspectId); }

        [Field]
        public string AspectName { get => _aspectName; set => UpdateProperty(ref value, ref _aspectName); }

        [Field]
        public string Description { get => _description; set => UpdateProperty(ref value, ref _description); }

        [Field]
        public string Color { get => _colorHex; set => UpdateProperty(ref value, ref _colorHex); }

        [Field]
        public double Orbit { get => _orbit; set => UpdateProperty(ref value, ref _orbit); }

        [Field]
        public bool IsMajor { get => _isMajor; set => UpdateProperty(ref value, ref _isMajor); }

        [Field]
        public bool IsDashed { get => _isDashed; set => UpdateProperty(ref value, ref _isDashed); }

        [Field]
        public double NatalTollerance { get => _natalTollerance; set => UpdateProperty(ref value, ref _natalTollerance); }

        [Field]
        public string URI { get => _uri; set => UpdateProperty(ref value, ref _uri); }

        [FK]
        public Energy Energy { get => _energy; private set => UpdateProperty(ref value, ref _energy); }

        public double OrbDiff { get; protected set; }
        public string OrbDiffInDegree { get; protected set; } = string.Empty;
        public string Info => $"{PointA}{TryFetchHouse(PointA)}{this} {PointB}{TryFetchHouse(PointB)} ({OrbDiffInDegree})";
        public string FullInfo => $"{PointA}{IsRetrograde()}{TryFetchHouse(PointA)}{this} {PointB}{TryFetchHouse(PointB)} ({OrbDiffInDegree}) on {DateOf.ToString("dd/MM/yyyy")}";
        public IPoint PointA { get; set; } = null!;
        public IPoint PointB { get; set; } = null!;
        public Brush Brush { get; protected set; } = Brushes.Black;
        public DateTime DateOf { get; set; }
        public int TransitBundle { get; set; }
        public TransitType TransitType { get; set; } = new(1);
        public bool IsConjunction => Orbit == 0;
        public bool IsInCuspid => OrbDiff >= -2.5 && OrbDiff <= 2.5;
        public Aspect() 
        {
            AfterUpdate += OnAfterUpdate;
        }

        public Aspect(Int64 id) : this() => _aspectId = id;
        public Aspect(DbDataReader reader) : this()
        {
            _aspectId = reader.GetInt64(0);
            _aspectName = reader.GetString(1);
            _description = reader.GetString(2);
            _orbit = reader.GetDouble(3);
            _isMajor = reader.GetBoolean(4);
            _uri = reader.GetString(5);
            _colorHex = reader.GetString(6);
            _natalTollerance = reader.GetDouble(7);
            _isDashed = reader.GetBoolean(8);
            _energy = new(reader.GetInt32(9));
        }

        public bool IsSame(Aspect? aspect)
        {
            return aspect != null && this.AspectId == aspect.AspectId
                && this.PointA.PointName.Equals(aspect.PointA.PointName)
                && this.PointAIsRetrograde() == aspect.PointAIsRetrograde()
                && this.PointB.PointName == aspect.PointB.PointName
                && this.DateOf == aspect.DateOf
                ;
        }

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Color))) 
            {
                BuildBrush();
            }
        }

        public bool PointAIsStar() => PointA is Star;
        public bool PointBIsHouse() => PointB is House;
        public bool PointAIsRetrograde()
        {
            if (PointA is Star star) return star.IsRetrograde;
            return false;
        }

        public string IsRetrograde()
        {
            return PointAIsRetrograde() ? " R " : string.Empty; 
        }

        private static string TryFetchHouse(IPoint point) 
        {
            if (point is Star star && star.House != null) return $" in {star.House} ";
            return " ";
        }
        public void BuildBrush() 
        {
            Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_colorHex));
        }

        public void CalculateOrbDiff()
        {
            OrbDiff = Math.Round(Math.Abs(_diff) - Orbit, 2);
            if (_diff < 0) OrbDiff = -OrbDiff;

            int degrees = (int)OrbDiff;
            int minutes = Math.Abs((int)((OrbDiff - degrees) * 60));
            
            OrbDiffInDegree = $"{degrees}°{minutes}'";

            if (degrees == 0 && OrbDiff < 0) OrbDiffInDegree = $"-{degrees}°{minutes}'";
            else OrbDiffInDegree = $"{degrees}°{minutes}'";            
        }

        public Aspect Clone(double diff) 
        {
            return new()
            {
                _aspectId = this.AspectId,
                _aspectName = this.AspectName,
                _description = this.Description,
                _orbit = this.Orbit,
                _isMajor = this.IsMajor,
                _isDashed = this.IsDashed,
                _colorHex = this._colorHex,
                PointA = this.PointA,
                PointB = this.PointB,
                _natalTollerance = this.NatalTollerance,
                _uri = this.URI,
                _energy = this.Energy,
                DateOf = this.DateOf,
                _diff = diff
            };
        }

        public override string ToString()
        {
            return AspectName;
        }
    }
}