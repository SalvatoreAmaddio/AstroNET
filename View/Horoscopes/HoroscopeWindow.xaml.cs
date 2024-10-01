﻿using FrontEnd.Dialogs;
using System.Windows;
using WpfApp1.model;
using Xceed.Wpf.Toolkit;

namespace WpfApp1.View
{
    public partial class HoroscopeWindow : CommonHoroscopeDateWindow
    {

        private TimeSpan? _inputTime = DateTime.Now.TimeOfDay;

        public TimeSpan? InputTime
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
            DataContext = this;
        }

        protected override async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (InputTime == null)
            {
                Failure.Allert("Please provide a valid time");
                return;
            }

            base.OnButtonClick(sender, e);

            if (StopRun) return;

            await Task.Run(() =>
            {
                SubjectSky.CalculateHoroscope(InputDate!.Value, InputTime.Value, SelectedCity!);
            });

            IsLoading = false;
            //Application.Current.Dispatcher.Invoke(() => 
            //{
            //    ChartOpener.OpenChart($"{SubjectSky?.Person?.ToString()}", SubjectSky!, SkyType.Horoscope);
            //});
        }
    }
}