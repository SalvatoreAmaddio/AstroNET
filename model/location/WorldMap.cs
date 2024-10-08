using AstroNETLibrary.Atlas;
using Backend.Database;
using FrontEnd.Dialogs;
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

                try 
                {
                    bool timeZoneIsFetched = await loc.FetchTimeZoneAsync(username);
                    if (!timeZoneIsFetched) continue;
                }
                catch (Exception ex) 
                {
                    Failure.Allert(ex.Message);
                    continue;
                }


                yield return loc;
            }
        }
    }
}