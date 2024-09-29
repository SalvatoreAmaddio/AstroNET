﻿using FrontEnd.Dialogs;
using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
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
            if (InputYear <= SubjectSky.Year)
            {
                Failure.Allert("Input Year field must be greater than the Year of Birth.");
                return;
            }

            base.OnButtonClick(sender, e);

            if (StopRun) return;

            SkyEvent subjectSky = ((ChartViewContainer)Owner.Content).Sky;

            PositionCalculator calculator = new(subjectSky);

            (DateTime returnDate, TimeSpan returnTime) = await Task.Run(() => 
            {
                return calculator.CalculateSunReturn(InputYear, SelectedCity);
            });

            SkyEvent returnSky = subjectSky.CalculateReturn(returnDate, returnTime, SelectedCity);

            IsLoading = false;
            ChartOpener.OpenComparedChart($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyType);
        }
    }
}