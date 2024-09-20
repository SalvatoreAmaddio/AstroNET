using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class MoonReturnWindow : CommonHoroscopeWindow
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

        public MoonReturnWindow() : base() 
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
            PositionCalculator c = new(subjectSky);

            (DateTime returnDate, TimeSpan returnTime) = await Task.Run(()=>c.MoonReturn(DateTime.Today, SelectedCity));

            SkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity, SkyType.MoonReturn);
            IsLoading = false;

            ChartOpener.OpenSinastry($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyType);
        }
    }
}