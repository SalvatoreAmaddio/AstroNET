using AstroNETLibrary.Sky;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    [Table(nameof(SavedCharts))]
    public class SavedCharts : AbstractModel<SavedCharts>
    {
        #region back props
        private Int64 _savedChartsId;
        private Person _person = null!;
        private DateTime _dateof;
        private TimeSpan _timeof;
        private City _city = null!;
        private string _notes = string.Empty;
        private int _skyTypeId = 2;
        #endregion

        #region Properties
        [PK]
        public Int64 SavedChartsId 
        {
            get => _savedChartsId;
            set => UpdateProperty(ref value, ref _savedChartsId);
        }

        [FK]
        public Person Person 
        {
            get => _person;
            set => UpdateProperty(ref value, ref _person);
        }

        [Field]
        public DateTime Dateof 
        {
            get => _dateof;
            set => UpdateProperty(ref value, ref _dateof);
        }

        [Field]
        public TimeSpan Timeof
        {
            get => _timeof;
            set => UpdateProperty(ref value, ref _timeof);
        }

        [FK]
        public City City
        {
            get => _city;
            set => UpdateProperty(ref value, ref _city);
        }

        [Field]
        public string Notes
        {
            get => _notes;
            set => UpdateProperty(ref value, ref _notes);
        }

        [Field]
        public int SkyTypeId
        {
            get => _skyTypeId;
            set => UpdateProperty(ref value, ref _skyTypeId);
        }
        #endregion

        public SavedCharts() { }

        public SavedCharts(AbstractSkyEvent sky) 
        {
            _person = (Person)sky.Person;
            _city = (City)sky.SkyInfo.City;
            _dateof = sky.SkyInfo.LocalDateTime;
            _timeof = sky.SkyInfo.LocalTime;

            switch (sky.SkyInfo.SkyType)
            {
                case SkyType.Horoscope:
                    _skyTypeId = 2;
                    break;
                case SkyType.SunReturn:
                    _skyTypeId = 4;
                    break;
                case SkyType.MoonReturn:
                    _skyTypeId = 5;
                    break;
            }
        }

        public SavedCharts(long id) : this()
        {
            _savedChartsId = id;
        }

        public SavedCharts(DbDataReader reader) 
        {
            _savedChartsId = reader.GetInt64(0);
            _person = new(reader.GetInt64(1));
            _dateof = reader.GetDateTime(2);
            _timeof = TimeSpan.Parse(reader.GetString(3));
            _city = new(reader.GetInt64(4));
            _notes = reader.GetString(5);
            _skyTypeId = reader.GetInt32(6);
        }
    }
}