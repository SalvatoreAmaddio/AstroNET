﻿using FrontEnd.Dialogs;
using System.Windows;
using AstroNETLibrary.Sky;
using AstroNET.View.Customs;

namespace AstroNET.View
{
    public partial class SunReturnWindow : CommonHoroscopeWindow
    {

        private int _inputYear = DateTime.Today.Year;

        public int InputYear
        {
            get => _inputYear;
            set
            {
                _inputYear = value;
                OnPropertyChanged(nameof(InputYear));
            }
        }

        public SunReturnWindow() : base()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (InputYear <= SubjectSky.SkyInfo.Year)
            {
                Failure.Allert("Input Year field must be greater than the Year of Birth.");
                return;
            }

            base.OnButtonClick(sender, e);

            if (StopRun) return;

            SkyEvent subjectSky = (SkyEvent)((ChartViewContainer)Owner.Content).Sky;

            if (subjectSky!.Person!.UnknownTime)
            {
                Failure.Allert("Cannot calculate Return on Unknown Time Sky");
                return;
            }

            PositionCalculator calculator = new(subjectSky);


            try
            {
                cts = new CancellationTokenSource();

                (DateTime returnDate, TimeSpan returnTime) = await Task.Run(() =>
                {
                    return calculator.CalculateSunReturnAsync(InputYear, SelectedCity!, cts.Token);
                }, cts.Token);

                ReturnSkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity!);

                IsLoading = false;
                ChartOpener.OpenComparedChart($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyInfo.SkyType);
            }
            catch (OperationCanceledException)
            {
                Helper.DisplayTaskCancelled();
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}