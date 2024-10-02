using Backend.Database;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Model;
using MvvmHelpers;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Windows.Input;
using AstroNET.model;
using TimeZone = AstroNET.model.TimeZone;

namespace AstroNET.controller
{
    public class AtlasDownloaderListController : INotifyPropertyChanged
    {
        public ICommand DownloadCMD { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool _isLoading = false;
        private Location? _selectedLocation;
        private string _search = string.Empty;
        private CountryController _countryController = new();
        private RegionController _regionController = new();
        private CityController _cityController = new();
        private TimeZoneController _timeZoneController = new();
        public ObservableRangeCollection<Location> Locations { get; private set; } = new();
        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                PropertyChanged?.Invoke(this, new(nameof(SelectedLocation)));
            }
        }

        public string Search 
        {
            get => _search;
            set 
            { 
                _search = value;
                PropertyChanged?.Invoke(this, new(nameof(Search)));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new(nameof(IsLoading)));
            }
        }
        public AtlasDownloaderListController() 
        {
            DownloadCMD = new Command<Location>(Download);
        }
        
        private void Download(Location? location) 
        {
            SelectedLocation = location;
            Save(location);
        }

        private M InsertIfMissing<M>(Location location, AbstractFormController<M> controller, string selectQry, List<QueryParameter> para) where M : IAbstractModel, ILocation, new()
        {
            controller.CurrentRecord = (M?)controller.Db.Retrieve(selectQry, para).FirstOrDefault();

            if (controller.CurrentRecord == null)
            {
                M m = new M();
                m.ReadLocation(location);
                controller.CurrentRecord = m;
                controller.PerformUpdate();
            }

            return controller.CurrentRecord;
        }
        
        private void Save(Location? location) 
        {
            if (location == null) return;
            Country country = InsertIfMissing<Country>(location, _countryController, "SELECT * FROM Country WHERE CountryName = @countryName;", [new("countryName", location.CountryName)]);

            location.Country = country;

            Region region = InsertIfMissing<Region>(location, _regionController, "SELECT * FROM Region WHERE RegionName = @regionName;", [new("regionName", location.RegionName)]);

            location.Region = region;

            bool cityExist = _cityController.CityExits(location.CityName, region.RegionId);
            if (cityExist) 
            {
                Failure.Allert("This record already exists","Duplicate Record");
                return;
            }

            TimeZone timeZone = InsertIfMissing<TimeZone>(location, _timeZoneController, "SELECT * FROM TimeZone WHERE TimeZoneName = @timeZoneName;", [new("timeZoneName", location.TimeZoneId)]);

            location.TimeZ = timeZone;

            City city = new(location);
            _cityController.CurrentRecord = city;
            _cityController.PerformUpdate();
            SuccessDialog.Display("Record successfully downloaded!");
        }
        
        public async Task OnSearchAsync()
        {
            IsLoading = true;
            Locations.Clear();

            JToken? responseToken = await WorldMap.FetchData(Search, 10);

            if (responseToken == null) return;

            await foreach (Location? location in WorldMap.ExtractLocations(responseToken))
            {
                Locations.Add(location);
            }
            IsLoading = false;
        }
    }
}