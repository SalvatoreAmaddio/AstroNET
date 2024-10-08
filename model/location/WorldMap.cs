using AstroNETLibrary.Atlas;
using Backend.Database;
using FrontEnd.Dialogs;
using MvvmHelpers;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace AstroNET.model
{
    public class WorldMap
    {
        private static readonly IAbstractDatabase? db = DatabaseManager.Find<City>();
        private static string username = "salvatore";
        //private static string username = "annahut";

        public static async Task<JToken?> FetchData(string cityName, int maxRows = 1000)
        {
            using (HttpClient client = new())
            {
                string searchUrl = $"http://api.geonames.org/searchJSON?name_startsWith={cityName}&maxRows={maxRows}&featureClass=P&username={username}";
                HttpResponseMessage response = await client.GetAsync(searchUrl);
                string searchResponse = await client.GetStringAsync(searchUrl);
                return JObject.Parse(searchResponse)["geonames"];
            }
        }

        public static async Task<bool> FetchTimeZoneAsync(Location loc)
        {
            string timezoneResponse;

            using (HttpClient client = new())
            {
                try
                {
                    string timezoneUrl = $"http://api.geonames.org/timezoneJSON?lat={loc.Latitude}&lng={loc.Longitude}&username={username}";
                    timezoneResponse = await client.GetStringAsync(timezoneUrl);
                }
                catch (Exception e)
                {
                    Failure.Allert(e.Message);
                    return false;
                }
            }

            JToken timezoneJson = JToken.Parse(timezoneResponse);
            if (timezoneJson["timezoneId"]?.ToString() == null)
                return false;

            try
            {
                loc.TimeZoneId = timezoneJson["timezoneId"]?.ToString();
                return loc.CalculateTimeZone();
            }
            catch
            {
                return false;
            }
        }

        public static async Task<IEnumerable<Location>> ExtractLocations2(JToken tokens)
        {
            ObservableRangeCollection<Location> locs = new();
            foreach (JToken item in tokens)
            {
                Location loc = new()
                {
                    CityInfo = item
                };
                loc.Build();

                bool any = db.Retrieve("SELECT City.* FROM City INNER JOIN Region ON City.RegionId = Region.RegionId WHERE CityName = @cityName AND RegionName = @regionName;", [new("cityName", loc.CityName), new("regionName", loc.RegionName)]).Any();

                if (any) continue;

                bool timeZoneIsFetched = await FetchTimeZoneAsync(loc);

                if (!timeZoneIsFetched) continue;

                locs.Add(loc);
            }
            return locs;
        }

        public static async IAsyncEnumerable<Location> ExtractLocations(JToken tokens)
        {
            foreach (JToken item in tokens)
            {
                Location loc = new()
                {
                    CityInfo = item
                };
                loc.Build();

                bool any = db.Retrieve("SELECT City.* FROM City INNER JOIN Region ON City.RegionId = Region.RegionId WHERE CityName = @cityName AND RegionName = @regionName;", [new("cityName", loc.CityName), new("regionName", loc.RegionName)]).Any();

                if (any) continue;

                bool timeZoneIsFetched = await FetchTimeZoneAsync(loc);

                if (!timeZoneIsFetched) continue;

                yield return loc;
            }
        }

        public static async IAsyncEnumerable<Location?> FindAll(string cityName, int maxRows = 1000)
        {

            using (HttpClient client = new())
            {
                string searchUrl = $"http://api.geonames.org/searchJSON?name_startsWith={cityName}&maxRows={maxRows}&featureClass=P&username={username}";
                HttpResponseMessage response = await client.GetAsync(searchUrl);
                string searchResponse = await client.GetStringAsync(searchUrl);
                JObject? searchJson = JObject.Parse(searchResponse);
                if (searchJson == null || searchJson["geonames"] == null)
                {
                    Failure.Allert(searchResponse);
                    yield return null;
                }

                foreach (var item in searchJson["geonames"])
                {
                    Location loc = new()
                    {
                        CityInfo = item ?? throw new NullReferenceException()
                    };
                    loc.Build();

                    var timezoneUrl = $"http://api.geonames.org/timezoneJSON?lat={loc.Latitude}&lng={loc.Longitude}&username={username}";
                    var timezoneResponse = await client.GetStringAsync(timezoneUrl);

                    JObject timezoneJson = JObject.Parse(timezoneResponse);
                    if (timezoneJson["timezoneId"]?.ToString() == null)
                    {
                        continue;
                    }
                    loc.TimeZoneId = timezoneJson["timezoneId"]?.ToString() ?? throw new Exception("Time zone not available");
                    bool res = loc.CalculateTimeZone();
                    if (!res) continue;
                    yield return loc;
                }
            }
        }
    }
}