﻿using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AstroNET.model;
using Brushes = System.Windows.Media.Brushes;

namespace AstroNET.View
{
    public static class Extenstion
    {
        public static void ReplaceSky(this Window win, AbstractSkyEvent sky) =>
        ((ChartViewContainer)win.Content).Sky = sky;

        public static AbstractSkyEvent? GetSky(this Window win)
        {
            try
            {
                return ((ChartViewContainer)win.Content).Sky;
            }
            catch
            {
                return null;
            }
        }
    }

    public class ChartOpener
    {
        public static void OpenChart(string title, SkyEvent sky, SkyType skyType)
        {
            Helper.GetActiveWindow()?.Close();

            if (sky.Horoscope != null)
                OpenChartWindow($"{title}", skyType, new ChartView() { Sky = sky }, false);
            else
                OpenChartWindow($"{title}", skyType, new ChartViewContainer() { Sky = sky }, false);
        }

        public static void OpenComparedChart(SinastryBundle sinastryBundle)
        {

            Grid chartGrid = CreateChartGrid(sinastryBundle.Sky1, sinastryBundle.Sky2);

            StackPanel infoStackPanel = new();

            SinastryChart sinastryChart = new()
            {
                SinastryBundle = sinastryBundle
            };

            infoStackPanel.Children.Add(sinastryChart);

            Helper.GetActiveWindow()?.Close();

            OpenChartWindow(sinastryBundle.Title, SkyType.Sinastry, CreateBackgroundWindow(chartGrid, infoStackPanel));
        }

        public static void OpenComparedChart(string? title, SkyEvent sky1, AbstractSkyEvent sky2, SkyType skyType)
        {
            Helper.GetActiveWindow()?.Close();

            Grid chartGrid = CreateChartGrid(sky1, sky2);

            StackPanel infoStackPanel = new();

            StackPanel backgroundWindow = CreateBackgroundWindow(chartGrid, infoStackPanel);

            if (sky2 is ReturnSkyEvent returnSky)
                CalculateReturnAsc(backgroundWindow, returnSky);

            OpenChartWindow(title, skyType, backgroundWindow);
        }

        private static void ChartWindowClosed(object? sender, EventArgs e)
        {
            Window? win = (Window?)sender;

            AbstractSkyEvent? abstractSky = win?.GetSky();

            if (abstractSky is SkyEvent skyEvent)
                skyEvent.ClearHoroscope();

            win!.Closed -= ChartWindowClosed;
        }

        private static void CalculateReturnAsc(StackPanel backgroundWindow, ReturnSkyEvent returnSky)
        {
            bool warning = returnSky.WarnReturn(returnSky.HouseHostingReturnAsc!);

            Label label = new()
            {
                Content = $"Return ASC in {returnSky.HouseHostingReturnAsc}",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new(0, 0, 0, .5),
                Foreground = Brushes.Blue,
                Cursor = Cursors.Hand,
                Tag = returnSky.HouseHostingReturnAsc
            };

            label.MouseDown += OnLabelMouseDown;

            backgroundWindow.Children.Insert(0, label);

            if (warning)
                backgroundWindow.Children.Insert(1, CreateWarningLabel());

            backgroundWindow.Unloaded += OnBackgroundWindowUnloaded;
        }

        private static Label CreateWarningLabel()
        {
            return new Label()
            {
                Content = "WARNING",
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
        }
        private static void OnBackgroundWindowUnloaded(object sender, RoutedEventArgs e)
        {
            StackPanel backgroundWindow = (StackPanel)sender;
            backgroundWindow.Unloaded -= OnBackgroundWindowUnloaded;
            ((Label)backgroundWindow.Children[0]).MouseDown -= OnLabelMouseDown;
        }

        private static void OnLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            House house = (House)((Label)sender).Tag;
            new Interpretation(LibrarySearch.SearchStarDescription(new Star(house), 4)).Show();
        }

        private static Grid CreateChartGrid(SkyEvent sky1, AbstractSkyEvent sky2)
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

            ChartViewContainer chart1 = new(true) { Sky = sky1 };
            ChartViewContainer chart2 = new(true) { Sky = sky2 };

            grid.Children.Add(chart1);
            grid.Children.Add(chart2);
            Grid.SetColumn(chart2, 1);
            return grid;
        }

        private static StackPanel CreateBackgroundWindow(Grid chartGrid, StackPanel infoStackPanel)
        {
            StackPanel backgroundWindow = new();
            backgroundWindow.Children.Add(chartGrid);
            backgroundWindow.Children.Add(infoStackPanel);
            return backgroundWindow;
        }

        private static void OpenChartWindow(string? title, SkyType skyType, object backgroundWindow, bool withinScrollViewer = true)
        {
            Window chartWindow = new()
            {
                Title = $"{title} - {skyType}",
                Content = (withinScrollViewer) ? new ScrollViewer() { Content = backgroundWindow } : backgroundWindow,
                Icon = new BitmapImage(new Uri("pack://application:,,,/assets/img/astrology.png")),
            };
            chartWindow.Closed += ChartWindowClosed;

            chartWindow.Show();
        }
    }
}