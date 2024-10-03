using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AstroNET.model;
using Brushes = System.Windows.Media.Brushes;
using FrontEnd.Dialogs;
using AstroNET.controller;

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
            {
                OpenChartWindow($"{title}", skyType, new ChartView() { Sky = sky, IsHoroscope = true }, false);
            }
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

        private static Button CreateSaveButton(AbstractSkyEvent sky2) 
        {
            Button save = new()
            {
                Padding = new(6, 1, 6, 20),
                ToolTip = "Save",
                Content = new Image()
                {
                    Source = Helper.LoadFromImages("save"),
                    Height = 20
                }
            };
            save.Tag = sky2;
            save.Click += OnSaveClicked;
            save.Unloaded += OnSaveUnloaded;
            return save;
        }

        private static void OnSaveUnloaded(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Unloaded -= OnSaveUnloaded;
            button.Click -= OnSaveClicked;
        }

        private static void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AbstractSkyEvent sky2 = (AbstractSkyEvent)button.Tag;

            if (sky2 is ReturnSkyEvent returnSky)
            {
                SavedChartControllerList controller = new();
                SavedCharts chart = new(sky2);
                chart.IsDirty = true;
                controller.SetCurrentRecord(chart);
                controller.PerformUpdate();
                SuccessDialog.Display();
            }
            else 
            {
                Failure.Allert("Cannot save Sinastry Chart");
            }
        }

        private static ToolBarTray CreateToolBarTray(AbstractSkyEvent sky2)
        {
            ToolBarTray tray = new() { IsLocked = true };
            ToolBar toolBar = new();
            toolBar.Items.Add(CreateSaveButton(sky2));
            tray.ToolBars.Add(toolBar);
            return tray;
        }

        private static Grid CreateChartGrid(SkyEvent sky1, AbstractSkyEvent sky2)
        {
            Grid grid = new();
            grid.RowDefinitions.Add(new() { Height = new(25) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });

            ChartView chart1 = new() { Sky = sky1, ToolBarHeight = 0 };
            ChartView chart2 = new() { Sky = sky2, ToolBarHeight = 0 };

            ToolBarTray tray = CreateToolBarTray(sky2);

            grid.Children.Add(tray);
            grid.Children.Add(chart1);
            grid.Children.Add(chart2);

            Grid.SetColumnSpan(tray, 2);
            Grid.SetRow(chart1, 1);
            Grid.SetRow(chart2, 1);
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