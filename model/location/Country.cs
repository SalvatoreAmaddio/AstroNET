using System.Data.Common;
using Backend.Database;
using Backend.Model;
using FrontEnd.Model;

namespace AstroNET.model
{
    public interface ICountry : ILocation
    {
        Int64 CountryId { get; set; }
        string CountryName { get; set; }
        string CountryCode { get; set; }

    }

    [Table(nameof(Country))]
    public class Country : AbstractModel<Country>, ICountry
    {
        private Int64 _countryId;
        private string _countryName = string.Empty;
        private string _countryCode = string.Empty;

        [PK]
        public Int64 CountryId { get => _countryId; set => UpdateProperty(ref value, ref _countryId); }
        [Field]
        public string CountryName { get => _countryName; set => UpdateProperty(ref value, ref _countryName); }
        [Field]
        public string CountryCode { get => _countryCode; set => UpdateProperty(ref value, ref _countryCode); }

        public Country() { }
        public Country(Int64 id) => _countryId = id;
        public Country(DbDataReader reader) 
        {
            _countryId = reader.GetInt64(0);
            _countryName = reader.GetString(1);
            _countryCode = reader.GetString(2);
        }

        public void Build()
        {
            Country _country = DatabaseManager.Find<Country>()!.MasterSource.Cast<Country>().First(s => s.Equals(this));
            _countryName = _country.CountryName;
            _countryCode = _country.CountryCode;
        }

        public void ReadLocation(Location location)
        {
            CountryName = location.CountryName;
            CountryCode = location.CountryCode;
        }
        public override string? ToString() => $"{CountryName}";
    }
}
