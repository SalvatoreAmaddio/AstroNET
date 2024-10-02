using FrontEnd.ExtensionMethods;
using System.Windows;
using System.Windows.Input;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class PersonForm : Window
    {
        public PersonForm()
        {
            InitializeComponent();
            this.SetController(new PersonController());
        }

        public PersonForm(Person? person, bool isTodaySky = false, bool isNewSky = false) : this()
        {
            this.GetController<PersonController>()!.IsNewSky = isNewSky;

            if (isTodaySky)
            {
                this.GetController<PersonController>()!.CurrentRecord = person;
            }
            else
            {
                this.GetController<PersonController>()!.GoAt(person);
            }
            this.GetController<PersonController>()!.CityListController._search = person?.City?.CityName;
        }

        private void OnLabelClikced(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new AtlasDownloadForm().Show();
        }
    }
}