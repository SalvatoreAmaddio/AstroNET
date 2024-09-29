using FrontEnd.Utils;
using System;
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
        
        private static void WriteSinastria(ref StackPanel infoStackPanel, SkyEvent sky1, SkyEvent sky2) 
        {

            IEnumerable<Aspect> aspects = sky1.CalculateSinastry(sky2);
            IEnumerable<Aspect> aspects2 = sky2.CalculateSinastry(sky1);

            IEnumerable<Star>? stars1 = sky1.StarsInPartnerHouses(sky2);
            IEnumerable<Star>? stars2 = sky2.StarsInPartnerHouses(sky1);

            SinastryAspects sinastryAspects = new();
            sinastryAspects.Person1.Content = sky1.Person;
            sinastryAspects.Person2.Content = sky2.Person;

            sinastryAspects.Person10.Content = sky2.Person;
            sinastryAspects.Person20.Content = sky1.Person;

            sinastryAspects.Lista.ItemsSource = aspects;
            sinastryAspects.Lista2.ItemsSource = aspects2;

            sinastryAspects.Person1Zodiac.Content = $"{sky1.Person} Stars in {sky2.Person} Houses";
            sinastryAspects.Person2Zodiac.Content = $"{sky2.Person} Stars in {sky1.Person} Houses";

            sinastryAspects.Lista3.ItemsSource = stars1;
            sinastryAspects.Lista4.ItemsSource = stars2;

            if (!sky2.Person.UnknownTime) 
            {
                IEnumerable<ElementGroupKey>? occupiedHouses1 = stars1?.GroupBy(s => s.House)
                                            .Select(s => new ElementGroupKey(s.Key.PointName, s.Count()))
                                            .OrderByDescending(s => s.Count).ToList();

                sinastryAspects.OccupiedHouses1.ItemsSource = occupiedHouses1;
                sinastryAspects.Stelliums1.ItemsSource = occupiedHouses1?.Where(s => s.Count >= 3).ToList();
            }

            if (!sky1.Person.UnknownTime)
            {
                IEnumerable<ElementGroupKey>? occupiedHouses2 = stars2?.GroupBy(s => s.House)
                                            .Select(s => new ElementGroupKey(s.Key.PointName, s.Count()))
                                            .OrderByDescending(s => s.Count).ToList();

                sinastryAspects.OccupiedHouses2.ItemsSource = occupiedHouses2;
                sinastryAspects.Stelliums2.ItemsSource = occupiedHouses2?.Where(s => s.Count >= 3).ToList();
            }

            infoStackPanel.Children.Add(sinastryAspects);
        }

        public static void OpenComparedChart(string? title, SkyEvent sky1, SkyEvent sky2, SkyType skyType)
        {
            Window? currentWindow = Helper.GetActiveWindow();
            currentWindow?.Close();

            StackPanel backgroundWindow = new();

            Grid chartGrid = CreateChartGrid(sky1, sky2);

            StackPanel infoStackPanel = new();

            if (sky2.SkyType == SkyType.SunReturn || sky2.SkyType == SkyType.MoonReturn) 
            {
                CalculateReturnAsc(backgroundWindow, sky1, sky2);
            }

            backgroundWindow.Children.Add(chartGrid);
            backgroundWindow.Children.Add(infoStackPanel);

            if (skyType == SkyType.Sinastry)
            {
                WriteSinastria(ref infoStackPanel, sky1, sky2);
            }

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