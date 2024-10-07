using System.Data.Common;
using Backend.Database;
using Backend.Model;
using FrontEnd.Model;

namespace AstroNET.model
{
    public interface IRegion : ILocation
    {
        Int64 RegionId { get; set; }
        string RegionName { get; set; }
        ICountry GetCountry();
    }

    [Table(nameof(Region))]
    public class Region : AbstractModel<Region>, IRegion
    {
        private Int64 _regionId;
        private string _regionName = string.Empty;
        private Country _country = null!;

        [PK]
        public Int64 RegionId { get => _regionId; set => UpdateProperty(ref value, ref _regionId); }
        [Field]
        public string RegionName { get => _regionName; set => UpdateProperty(ref value, ref _regionName); }
        [FK]
        public Country Country { get => _country; set => UpdateProperty(ref value, ref _country); }

        public Region() { }
        public Region(Int64 id) => _regionId = id;
        public Region(DbDataReader reader)
        {
            _regionId = reader.GetInt64(0);
            _regionName = (reader.IsDBNull(1)) ? string.Empty : reader.GetString(1);
            _country = new(reader.GetInt64(2));
        }
        public void Build()
        {
            Region _region = DatabaseManager.Find<Region>()!.MasterSource.Cast<Region>().First(s => s.Equals(this));
            _regionName = _region.RegionName;
            _country = _region.Country;
            _country.Build();
        }

        public void ReadLocation(Location location)
        {
            RegionName = location.RegionName;
            Country = (Country)location.Country;
        }

        public ICountry GetCountry() => _country;
        public override string? ToString() => $"{RegionName}";
    }
}