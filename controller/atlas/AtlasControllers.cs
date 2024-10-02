using FrontEnd.Controller;
using AstroNET.model;
using TimeZone = AstroNET.model.TimeZone;

namespace AstroNET.controller
{
    public class CountryController : AbstractFormController<Country>
    {
    }

    public class CityController : AbstractFormController<City>
    {
        public bool CityExits(string cityName, Int64 regionId)
        {
            return Db.Retrieve("SELECT * FROM City WHERE CityName = @cityName AND RegionId = @regionId;", [new("cityName", cityName),new("regionId", regionId)]).Any();
        }
    }

    public class RegionController : AbstractFormController<Region>
    {
    }

    public class TimeZoneController : AbstractFormController<TimeZone>
    {
    }
}
