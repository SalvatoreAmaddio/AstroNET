using Backend.Database;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Forms;
using FrontEnd.Source;
using System.Windows;
using System.Windows.Input;
using AstroNET.model;
using AstroNET.View;
using FrontEnd.Events;
using FrontEnd.Utils;
using AstroNETLibrary.Sky;

namespace AstroNET.controller
{
    public class PersonController : AbstractFormController<Person>
    {
        public CityListController CityListController { get; } = new();
        public RecordSource<Gender> Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public ICommand CalculateSkyCMD { get; }
        public bool IsNewSky = false;
        public ICommand FilePickedCMD { get; }
        public ICommand OpenSunReturnsCMD { get; }
        public ICommand OpenMoonReturnsCMD { get; }
        public ICommand OpenHoroscopesCMD { get; }
        public PersonController()
        {
            OpenSunReturnsCMD = new CMD(OpenSunReturns);
            OpenMoonReturnsCMD = new CMD(OpenMoonReturns);
            OpenHoroscopesCMD = new CMD(OpenHoroscopes);
            FilePickedCMD = new Command<FilePickerCatch>(PickPicture);
            CalculateSkyCMD = new CMD(OpenSkyChart);
            CheckIsDirtyOnClose = false;
            AfterUpdate += OnAfterUpdate;
            WindowLoaded += OnWindowLoaded;
        }

        private void OpenSavedCharts(int skyTypeId, string title) 
        {
            if (CurrentRecord == null) return;
            if (CurrentRecord.IsNewRecord()) 
            {
                Failure.Allert("The record must be saved before opening saved charts");
                return;
            }
            SkyEvent? sky = CalculateSky();
            if (sky == null) return;
            new SavedChartsList(skyTypeId, sky) { Title = title}.ShowDialog();
        }

        private void OpenSunReturns() 
        {
            OpenSavedCharts(4, "Sun Returns");
        }

        private void OpenMoonReturns()
        {
            OpenSavedCharts(5, "Moon Returns");
        }

        private void OpenHoroscopes()
        {
            OpenSavedCharts(2, "Horoscopes");
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
            {
                await SetCityOnFilter();
            }
        }

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            await SetCityOnFilter();
            CurrentRecord?.Clean();
        }

        private async Task SetCityOnFilter() 
        {
            CityListController._search = Search;
            await CityListController.RunCitySearchAsync();
            City? city = CityListController.CurrentRecord;
            if (city != null && CurrentRecord != null)
                CurrentRecord.City = city;
        }

        private void PickPicture(FilePickerCatch? filePicked)
        {
            string? newFilePath = Helper.PickPicture($"{CurrentRecord?.PersonId}_{CurrentRecord?.FirstName}_{CurrentRecord?.LastName}_PROFILE_PICTURE", "personPictures", this, filePicked);

            if (newFilePath != null && CurrentRecord != null)
                CurrentRecord.PictureURL = newFilePath;
        }

        private SkyEvent? CalculateSky() 
        {
            DateTime? date = CurrentRecord?.DOB;
            TimeSpan? time = CurrentRecord?.TOB;
            City? city = CurrentRecord?.City;

            if (date == null)
            {
                Failure.Allert("The Date field cannot be empty");
                return null;
            }

            if (time == null)
            {
                Failure.Allert("The Time field cannot be empty");
                return null;
            }

            if (city == null)
            {
                Failure.Allert("The City field cannot be empty");
                return null;
            }

            CurrentRecord?.City.Build();
            return new(CurrentRecord!, this);
        }

        private void OpenSkyChart()
        {

            SkyEvent? sky = CalculateSky();

            if (sky == null) return;

            if (IsNewSky)
            {
                ChartOpener.OpenChart($"{CurrentRecord}", sky, sky.SkyInfo.SkyType);
            }
            else
            {
                Window? currentWindow = UI as Window;
                Window? parentWindow = currentWindow?.Owner;

                if (CurrentRecord != null)
                    parentWindow?.ReplaceSky(new SkyEvent(CurrentRecord, this));

                currentWindow?.Close();
            }
        }
    }
}