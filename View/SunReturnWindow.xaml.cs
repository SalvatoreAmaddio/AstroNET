using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class SunReturnWindow : CommonHoroscopeWindow
    {

        private bool _isLoading = false;
        private int _inputYear = DateTime.Today.Year;
        
        public bool IsLoading 
        {
            get => _isLoading;
            set 
            { 
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public int InputYear 
        {
            get => _inputYear;
            set 
            { 
                _inputYear = value;
                OnPropertyChanged(nameof(InputYear));     
            }
        }

        public SunReturnWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Search = SelectedCity.CityName;
        }

        protected override async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            SelectedCity?.Build();

            SkyEvent subjectSky = ((ChartViewContainer)Owner.Content).Sky;

            PositionCalculator calculator = new(subjectSky);

            (DateTime returnDate, TimeSpan returnTime) = await Task.Run(() => 
            {
                return calculator.CalculateSunReturn(InputYear, SelectedCity);
            });

            SkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity);

            IsLoading = false;
            ChartOpener.OpenSinastry($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyType);
        }
    }
}