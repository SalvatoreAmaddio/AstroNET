using FrontEnd.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1.model;

namespace WpfApp1.View
{
    public static class Extenstion 
    {
        public static void ReplaceSky(this Window win, SkyEvent sky) 
        {
            ((ChartViewContainer)win.Content).Sky = sky;
        }

        public static SkyEvent GetSky(this Window win)
        {
            return ((ChartViewContainer)win.Content).Sky;
        }
    }

    public class ChartOpener
    {
        public static void OpenChart(string title, SkyEvent sky, string skyType) 
        {
            Window? currentWindow = Helper.GetActiveWindow();

            Window chartWindow = new()
            {
                Title = $"{title} - {skyType}",
                Content = new ChartViewContainer() { Sky = sky },
                Icon = new BitmapImage(new Uri("pack://application:,,,/assets/img/astrology.png")),
                WindowState = WindowState.Maximized,
            };

            chartWindow.Closed += ChartWindowClosed;

            chartWindow.Show();
    
            currentWindow?.Close();
        }

        private static void ChartWindowClosed(object? sender, EventArgs e)
        {
            Window? win = (Window?)sender;
            win?.GetSky().ClearHoroscope();
            win.Closed -= ChartWindowClosed;
        }

        private static void CalculateReturnAsc(StackPanel stack, SkyEvent subjectSky, SkyEvent returnSky)
        {
            House returnAsc = returnSky.Houses.First();
            House inNatalHouse = returnAsc.PlaceInHouse(subjectSky);

            Label label = new()
            {
                Content = $"R. ASC in {inNatalHouse}",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new(0, 0, 0, .5)
            };

            stack.Children.Add(label);
        }

        private static Grid CreateChartGrid(SkyEvent sky1, SkyEvent sky2) 
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
        
        private static void WriteSinastria(ref StackPanel infoStackPanel, SinastryBundle sinastryBundle) 
        {
            SinastryChart sinastryChart = new();
            sinastryChart.SinastryBundle = sinastryBundle;
            //SinastryAspects sinastryAspects = new();
            //sinastryAspects.Person1.Content = sinastryBundle.Person1;
            //sinastryAspects.Person2.Content = sinastryBundle.Person2;

            //sinastryAspects.Person10.Content = sinastryBundle.Person2;
            //sinastryAspects.Person20.Content = sinastryBundle.Person1;

            //sinastryAspects.Lista.ItemsSource = sinastryBundle.Chart1Aspects;
            //sinastryAspects.Lista2.ItemsSource = sinastryBundle.Chart2Aspects;

            //sinastryAspects.Person1Zodiac.Content = $"{sinastryBundle.Person1} Stars in {sinastryBundle.Person2} Houses";
            //sinastryAspects.Person2Zodiac.Content = $"{sinastryBundle.Person2} Stars in {sinastryBundle.Person1} Houses";

            //sinastryAspects.Lista3.ItemsSource = sinastryBundle.Chart1Zodiac;
            //sinastryAspects.Lista4.ItemsSource = sinastryBundle.Chart2Zodiac;

            //sinastryAspects.OccupiedHouses1.ItemsSource = sinastryBundle.Chart1OccupiedHouses;
            //sinastryAspects.Stelliums1.ItemsSource = sinastryBundle.Chart1Stelliums;

            //sinastryAspects.OccupiedHouses2.ItemsSource = sinastryBundle.Chart2OccupiedHouses;
            //sinastryAspects.Stelliums2.ItemsSource = sinastryBundle.Chart2Stelliums;

            infoStackPanel.Children.Add(sinastryChart);
        }

        public static void OpenComparedChart(SinastryBundle sinastryBundle)
        {

            Grid chartGrid = CreateChartGrid(sinastryBundle.Sky1, sinastryBundle.Sky2);

            StackPanel infoStackPanel = new();

            WriteSinastria(ref infoStackPanel, sinastryBundle);

            Window? currentWindow = Helper.GetActiveWindow();
            currentWindow?.Close();

            OpenChartWindow(sinastryBundle.Title, SkyType.Sinastry, CreateBackgroundWindow(chartGrid, infoStackPanel));
        }

        public static void OpenComparedChart(string? title, SkyEvent sky1, SkyEvent sky2, SkyType skyType)
        {
            Window? currentWindow = Helper.GetActiveWindow();
            currentWindow?.Close();

            Grid chartGrid = CreateChartGrid(sky1, sky2);

            StackPanel infoStackPanel = new();

            StackPanel backgroundWindow = CreateBackgroundWindow(chartGrid, infoStackPanel);

            if (sky2.SkyType == SkyType.SunReturn || sky2.SkyType == SkyType.MoonReturn)
                CalculateReturnAsc(backgroundWindow, sky1, sky2);

            OpenChartWindow(title, skyType, CreateBackgroundWindow(chartGrid, infoStackPanel));
        }
    
        private static StackPanel CreateBackgroundWindow(Grid chartGrid, StackPanel infoStackPanel) 
        {
            StackPanel backgroundWindow = new();
            backgroundWindow.Children.Add(chartGrid);
            backgroundWindow.Children.Add(infoStackPanel);
            return backgroundWindow;
        }

        private static void OpenChartWindow(string? title, SkyType skyType, StackPanel backgroundWindow)
        {
            Window chartWindow = new()
            {
                Title = $"{title} - {skyType}",
                Content = new ScrollViewer() { Content = backgroundWindow },
                Icon = new BitmapImage(new Uri("pack://application:,,,/assets/img/astrology.png")),
            };

            chartWindow.Show();
        }
    }
}