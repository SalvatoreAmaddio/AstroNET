using System.Windows;

namespace WpfApp1.View
{
    public partial class HoroscopeWindow : CommonHoroscopeDateWindow
    {

        private TimeSpan _inputTime = DateTime.Now.TimeOfDay;

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

        protected override async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            SelectedCity?.Build();

            await Task.Run(() => SubjectSky.CalculateHoroscope(InputDate, InputTime, SelectedCity));
            
            IsLoading = false;

            ChartOpener.OpenChart(SubjectSky.Person.ToString(), SubjectSky, SubjectSky.Horoscope.SkyType.ToString());
        }
    }
}