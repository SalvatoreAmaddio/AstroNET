using Newtonsoft.Json.Linq;

namespace WpfApp1.model
{
    public class Location
    {
        public TimeZone TimeZ { get; set; } = null!;
        public Country Country { get; set; } = null!;
        public Region Region { get; set; } = null!;
        public string CountryCode { get; private set; } = string.Empty;
        public string RegionName { get; private set; } = string.Empty;
        public string CityName { get; private set; } = string.Empty;
        public string CountryName { get; private set; } = string.Empty;
        public JToken? CityInfo { get; set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string TimeZoneId { get; set; } = string.Empty;
        public TimeZoneInfo TimeZone { get; private set; } = TimeZoneInfo.Local;

        public Location() { }
        public Location(string cityName, double lat, double lng, string timeZoneId)
        {
            CityName = cityName;
            Latitude = lat;
            Longitude = lng;
            TimeZoneId = timeZoneId;
            CalculateTimeZone();
        }

        public void Build()
        {
            if (CityInfo is null)
                throw new NullReferenceException();

            CityName = CityInfo["name"]?.ToString().Trim() ?? throw new NullReferenceException();
            CountryName = CityInfo["countryName"]?.ToString().Trim() ?? throw new NullReferenceException();
            RegionName = CityInfo["adminName1"]?.ToString().Trim() ?? throw new NullReferenceException();
            Latitude = (double?)CityInfo["lat"] ?? throw new NullReferenceException();
            Longitude = (double?)CityInfo["lng"] ?? throw new NullReferenceException();
            CountryCode = CityInfo["countryCode"]?.ToString().Trim() ?? throw new NullReferenceException();
        }
        public bool CalculateTimeZone()
        {
            try
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public override string? ToString()
        {
            return $"{CityName} - {CountryName} - {RegionName} - {Latitude} - {Longitude} - {TimeZone}";
        }
    }

}
