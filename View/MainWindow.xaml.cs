using AstroNET;
using Backend.Database;
using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            string defaultCity = AstroNETSettings.Default.DefaultCity.ToLower();
            City city = DatabaseManager.Find<City>()!.MasterSource.Cast<City>().First(s => s.CityName.ToLower().Equals(defaultCity));
            city.Build();

            Person newBorn = new(DateTime.Today, DateTime.Now.TimeOfDay, city);

            NatalChartView.Sky = new SkyEvent(newBorn, false);
        }
    }
}