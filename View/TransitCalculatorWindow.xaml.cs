using System.ComponentModel;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class TransitCalculatorWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int _steps = 1;
        private string _searchLocation = string.Empty;
        private Star? _selectedStar;
        private City? _selectedCity;
        private bool _isLoading = false;
        private DateTime _selectedDate = DateTime.Today;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public Star? SelectedStar 
        {
            get => _selectedStar;
            set 
            {
                _selectedStar = value;
                OnPropertyChanged(nameof(SelectedStar));
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

        public string SearchLocation 
        {
            get => _searchLocation;
            set 
            { 
                _searchLocation = value;
                OnPropertyChanged(nameof(SearchLocation));
                FilterList();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public int Steps
        {
            get => _steps;
            set
            {
                _steps = value;
                OnPropertyChanged(nameof(Steps));
            }
        }

        public StarListController StarListController { get; } = new();
        public CityListController CityListController { get; } = new();
        public TransitCalculatorWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void FilterList()
        {
            IEnumerable<City> results = CityListController.Db.MasterSource.Cast<City>().Where(s => s.ToString().ToLower().StartsWith(SearchLocation.ToLower()));
            CityListController.RecordSource.ReplaceRecords(results);
            SelectedCity = CityListController.RecordSource.FirstOrDefault();
        }

        private void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this,new(propName));

        private void OnRunClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnLabelClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new AtlasDownloadForm().ShowDialog();
        }
    }
}