using Backend.Database;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Source;
using System.Windows;
using System.Windows.Input;
using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class PersonController : AbstractFormController<Person>
    {
        public CityListController CityListController { get; } = new();
        public RecordSource<Gender> Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public ICommand CalculateSkyCMD { get; }
        public bool IsNewSky = false;

        public PersonController()
        {
            CalculateSkyCMD = new CMD(CalculateSky);
            CheckIsDirtyOnClose = false;
        }

        private void CalculateSky()
        {
            DateTime? date = CurrentRecord?.DOB;
            TimeSpan? time = CurrentRecord?.TOB;
            City? city = CurrentRecord?.City;

            if (date == null)
            {
                Failure.Allert("The Date field cannot be empty");
                return;
            }

            if (time == null)
            {
                Failure.Allert("The Time field cannot be empty");
                return;
            }

            if (city == null)
            {
                Failure.Allert("The City field cannot be empty");
                return;
            }

            CurrentRecord?.City.Build();

            if (IsNewSky)
            {
                SkyEvent sky = new(CurrentRecord!, this);

                ChartOpener.OpenChart($"{CurrentRecord}", sky, sky.SkyType);
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