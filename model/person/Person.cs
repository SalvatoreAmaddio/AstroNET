using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;
using FrontEnd.Events;
using AstroNETLibrary.Person;
using AstroNETLibrary.Sky;
using AstroNETLibrary.Atlas;

namespace AstroNET.model
{

    [Table(nameof(Person))]
    public class Person : AbstractModel<Person>, IPerson
    {
        private Int64 _personId;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime _dob = DateTime.Today;
        private TimeSpan _tob = DateTime.Now.TimeOfDay;
        private Gender _gender = null!;
        private string _notes = string.Empty;
        private SkyEvent _skyEvent = null!;
        private bool _unknownTime = false;
        private City _city = new();
        private bool _deceased = false;
        private string _pictureURL = string.Empty;

        [PK]
        public Int64 PersonId { get => _personId; set => UpdateProperty(ref value, ref _personId); }
        [Field]
        public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }
        [Field]
        public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }

        [Mandatory]
        [Field]
        public DateTime DOB { get => _dob; set => UpdateProperty(ref value, ref _dob); }

        [Mandatory]
        [Field]
        public TimeSpan TOB { get => _tob; set => UpdateProperty(ref value, ref _tob); }

        [FK]
        public Gender Gender { get => _gender; set => UpdateProperty(ref value, ref _gender); }

        [Field]
        public string Notes { get => _notes; set => UpdateProperty(ref value, ref _notes); }

        [Field]
        public bool UnknownTime { get => _unknownTime; set => UpdateProperty(ref value, ref _unknownTime); }
        [FK]
        public City City { get => _city; set => UpdateProperty(ref value, ref _city); }

        [Field]
        public bool Deceased { get => _deceased; set => UpdateProperty(ref value, ref _deceased); }

        [Field]
        public string PictureURL { get => _pictureURL; set => UpdateProperty(ref value, ref _pictureURL); }

        public Person()
        {
            AfterUpdate += OnAfterUpdate;
            _gender = new(1);
        }

        public Person(long id) : this() 
        { 
            _personId = id;
        }

        public Person(DateTime date, TimeSpan tob, City city) : this()
        {
            _dob = date;
            _tob = tob;
            _city = city;
        }

        public Person(DateTime date, TimeSpan tob, City city, Gender gender) : this(date, tob, city)
        {
            _gender = gender;
        }

        public Person(DbDataReader reader) : this()
        {
            _personId = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _dob = reader.GetDateTime(3);
            _tob = TimeSpan.Parse(reader.GetString(4));
            _city = new(reader.GetInt64(5));
            _gender = new(reader.GetInt64(6));
            _notes = reader.GetString(7);
            _unknownTime = reader.GetBoolean(8);
            _deceased = reader.GetBoolean(9);
            _pictureURL = reader.GetString(10);
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(UnknownTime)))
            {
                if (UnknownTime)
                {
                    TOB = new TimeSpan(12, 0, 0);
                    return;
                }
            }

            if (e.Is(nameof(FirstName)))
            {
                _firstName = FirstName.FirstLetterCapital();
            }

            if (e.Is(nameof(LastName)))
            {
                _lastName = LastName.FirstLetterCapital();
            }
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }

        public ICity GetCity() => _city;
    }
}