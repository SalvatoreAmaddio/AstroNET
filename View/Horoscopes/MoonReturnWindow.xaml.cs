using FrontEnd.Dialogs;
using System.Windows;
using AstroNETLibrary.Sky;

namespace AstroNET.View
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

            SkyEvent subjectSky = (SkyEvent)((ChartViewContainer)Owner.Content).Sky;

            if (subjectSky!.Person!.UnknownTime)
            {
                Failure.Allert("Cannot calculate Return on Unknown Time Sky");
                return;
            }

            PositionCalculator calculator = new(subjectSky);

            (DateTime returnDate, TimeSpan returnTime) = await Task.Run(() => calculator.MoonReturn(InputDate!.Value, SelectedCity!));

            ReturnSkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity!, SkyType.MoonReturn);
            IsLoading = false;

            ChartOpener.OpenComparedChart($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyInfo.SkyType);
        }
    }
}