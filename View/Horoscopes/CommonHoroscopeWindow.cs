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
        private bool _isLoading = false;
        public SkyEvent SubjectSky => ((ChartViewContainer)Owner.Content).Sky;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

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

        public CommonHoroscopeWindow()
        {
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            Closed -= OnClosed;
            Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Search = SubjectSky.City.CityName;
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

        protected abstract void OnButtonClick(object sender, RoutedEventArgs e);
    }

    public abstract class CommonHoroscopeDateWindow : CommonHoroscopeWindow
    {
        private DateTime _inputDate = DateTime.Today;

        public DateTime InputDate
        {
            get => _inputDate;
            set
            {
                _inputDate = value;
                OnPropertyChanged(nameof(InputDate));
            }
        }
    }
}
