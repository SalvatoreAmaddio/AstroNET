using System.ComponentModel;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public abstract class CommonHoroscopeWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public CityListController CityListController { get; } = new();
        private string _search = string.Empty;
        private City? _selectedCity = new();

        public City? SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }

        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                OnPropertyChanged(nameof(Search));
                FilterList();
            }
        }

        private void FilterList()
        {
            IEnumerable<City> results = CityListController.Db.MasterSource.Cast<City>().Where(s => s.ToString().ToLower().StartsWith(Search.ToLower()));
            CityListController.RecordSource.ReplaceRecords(results);
            SelectedCity = CityListController.RecordSource.FirstOrDefault();
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new(propName));
        }

        public CommonHoroscopeWindow() 
        {
            _selectedCity = CityListController.RecordSource.First();
        }

        protected abstract void OnButtonClick(object sender, RoutedEventArgs e);
    }

    public partial class HoroscopeWindow : CommonHoroscopeWindow
    {

        private DateTime _inputDate = DateTime.Today;
        private TimeSpan _inputTime = DateTime.Now.TimeOfDay;

        public DateTime InputDate
        {
            get => _inputDate;
            set
            {
                _inputDate = value;
                OnPropertyChanged(nameof(InputDate));
            }
        }

        public TimeSpan InputTime
        {
            get => _inputTime;
            set
            {
                _inputTime = value;
                OnPropertyChanged(nameof(InputTime));
            }
        }

        public HoroscopeWindow() : base() 
        {
            InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedCity.Build();

            SkyEvent subjectSky = ((ChartViewContainer)Owner.Content).Sky;
            subjectSky.CalculateHoroscope(InputDate, InputTime, SelectedCity);

            ChartOpener.OpenChart(subjectSky.Person.ToString(), subjectSky, subjectSky.Horoscope.SkyType.ToString());
        }
    }
}