using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class MoonReturnWindow : CommonHoroscopeDateWindow
    {
        public MoonReturnWindow() : base() 
        {
            InitializeComponent();
            this.DataContext = this;
        }

        protected override async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            base.OnButtonClick(sender, e);

            if (StopRun) return;

            SkyEvent subjectSky = ((ChartViewContainer)Owner.Content).Sky;
            PositionCalculator calculator = new(subjectSky);

            (DateTime returnDate, TimeSpan returnTime) = await Task.Run(() => calculator.MoonReturn(InputDate!.Value, SelectedCity!));

            SkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity!, SkyType.MoonReturn);
            IsLoading = false;

            ChartOpener.OpenComparedChart($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyType);
        }
    }
}