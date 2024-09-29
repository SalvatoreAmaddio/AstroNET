using Backend.Database;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace WpfApp1.model
{
    [Table(nameof(Sign))]
    public class Sign : AbstractModel<Sign>
    {
        private Int64 _signId;
        private string _signName = string.Empty;
        private string _description = string.Empty;
        private Element _element = null!;
        private Triplicity _triplicity = null!;
        private Gender _gender = null!;
        private int _start;
        private int _end;
        private DateTime _startDay;
        private DateTime _endDay;
        private string _uri = string.Empty;

        [PK]
        public Int64 SignId { get => _signId; private set => UpdateProperty(ref value, ref _signId); }
        [Field]
        public string SignName { get => _signName; set => UpdateProperty(ref value, ref _description); }
        [Field]
        public string Description { get => _description; set => UpdateProperty(ref value, ref _description); }
        [FK]
        public Element Element { get => _element; private set => UpdateProperty(ref value, ref _element); }
        [FK]
        public Triplicity Triplicity {  get => _triplicity; private set => UpdateProperty(ref value, ref _triplicity); }
        [FK]
        public Gender Gender { get => _gender; private set => UpdateProperty(ref value, ref _gender); }
        [Field]
        public int Start { get => _start; private set => UpdateProperty(ref value, ref _start); }
        [Field]
        public int End { get => _end; private set => UpdateProperty(ref value, ref _end); }
        [Field]
        public DateTime StartDay { get => _startDay; private set => UpdateProperty(ref value, ref _startDay); }
        [Field]
        public DateTime EndDay { get => _endDay; private set => UpdateProperty(ref value, ref _endDay); }
        [Field]
        public string URI { get => _uri; set => UpdateProperty(ref value, ref _uri);}
        public string TimeExtension => $"({StartDay.Day} {StartDay.ToString("MMMM")} - {EndDay.Day} {EndDay.ToString("MMMM")})";
        public string GradeExtension => $"({Start}° - {End}°)";

        public Sign() { }

        public Sign(Int64 id) 
        {
            _signId = id;
        }
        
        public void Compile() 
        {
            Sign sign = DatabaseManager.Find<Sign>()!.MasterSource.Cast<Sign>().First(s => s.SignId == _signId)!;
            _signName = sign.SignName;
            _uri = sign.URI;
            _triplicity = sign.Triplicity;
            _element = sign.Element;
            _gender = sign.Gender;
            _startDay = sign.StartDay;
            _endDay = sign.EndDay;
            _start = sign.Start;
            _end = sign.End;
            _description = sign.Description;
        }

        public Sign(DbDataReader reader) 
        {
            _signId = reader.GetInt64(0);
            _signName = reader.GetString(1);
            _description = reader.GetString(2);
            _element = new(reader.GetInt64(3));
            _triplicity = new(reader.GetInt64(4));
            _gender = new(reader.GetInt64(5));
            _start = reader.GetInt32(6);
            _end = reader.GetInt32(7);
            _startDay = reader.GetDateTime(8);
            _endDay = reader.GetDateTime(9);
            _uri = reader.GetString(10);
        }

        public void Build() 
        {
            Sign? _sign = DatabaseManager.Find<Sign>()?.MasterSource.Cast<Sign>().First(s => s.Equals(this));
            if (_sign == null) return;
            _signName = _sign.SignName;
            _uri = _sign.URI;
            _description = _sign._description;
            _element = _sign._element;
            _triplicity = _sign._triplicity;
            _gender = _sign._gender;
            _start = _sign._start;
            _end = _sign._end;
            _startDay = _sign._startDay;
            _endDay = _sign._endDay;
        }

        public override string ToString()
        {
            return $"{SignName}";
        }
    }
}