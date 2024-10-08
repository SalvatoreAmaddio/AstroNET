using System.Data.Common;
using AstroNETLibrary.Atlas;
using Backend.Database;
using Backend.Model;
using FrontEnd.Model;

namespace AstroNET.model
{
    [Table(nameof(City))]
    public class City : AbstractModel<City>, ICity
    {
        private Int64 _cityId;
        private string _cityName = string.Empty;
        private Region _region = null!;
        private double _latitude;
        private double _longitude;
        private TimeZone _timeZone = null!;

        [PK]
        public Int64 CityId { get => _cityId; set => UpdateProperty(ref value, ref _cityId); }

        [Field]
        public string CityName { get => _cityName; set => UpdateProperty(ref value, ref _cityName); }

        [Field]
        public double Latitude { get => _latitude; set => UpdateProperty(ref value, ref _latitude); }

        [Field]
        public double Longitude { get => _longitude; set => UpdateProperty(ref value, ref _longitude); }

        [FK]
        public Region Region { get => _region; set => UpdateProperty(ref value, ref _region); }

        [FK]
        public TimeZone TimeZone { get => _timeZone; set => UpdateProperty(ref value, ref _timeZone); }

        public string Coords => $"Long: {Math.Round(Longitude, 2)}, Lat: {Math.Round(Latitude, 2)}";

        public City() { }
        public City(Int64 id) => _cityId = id;
        public City(DbDataReader reader)
        {
            _cityId = reader.GetInt64(0);
            _cityName = reader.GetString(1);
            _latitude = reader.GetDouble(2);
            _longitude = reader.GetDouble(3);
            _region = new(reader.GetInt64(4));
            _timeZone = new(reader.GetInt64(5));
        }

        public City(Location location) => ReadLocation(location);
        public void Build()
        {
            City tempCity = DatabaseManager.Find<City>()!.MasterSource.Cast<City>().First(s => s.Equals(this));
            _region = tempCity.Region;
            _region.Build();
            _timeZone = tempCity.TimeZone;
            _timeZone.Build();
            _cityName = tempCity.CityName;
            _latitude = tempCity.Latitude;
            _longitude = tempCity.Longitude;
        }

        public ITimeZone GetTimeZone() => _timeZone;
        public IRegion GetRegion() => _region;
        public override string? ToString() => $"{CityName}";

        public void ReadLocation(Location location)
        {
            CityName = location.CityName;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Region = (Region)location.Region;
            TimeZone = (TimeZone)location.TimeZ;
        }
    }
}