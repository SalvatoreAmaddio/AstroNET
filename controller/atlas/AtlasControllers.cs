using FrontEnd.Controller;
using WpfApp1.model;
using TimeZone = WpfApp1.model.TimeZone;

namespace WpfApp1.controller
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
