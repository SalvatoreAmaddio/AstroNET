using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.model;
using System.ComponentModel;

namespace WpfApp1.View
{
    public partial class ChartViewContainer : Grid
    {
        public static RoutedUICommand OpenArchiveCMD = new (
        "Open Archive",
        nameof(OpenArchiveCMD),
        typeof(Window),
        new ()
        {
            new KeyGesture(Key.A, ModifierKeys.Control)
        });

        public static RoutedUICommand OpenSinastryCMD = new(
        "Open Sinastry",
        nameof(OpenSinastryCMD),
        typeof(Window),
        new()
        {
            new KeyGesture(Key.C, ModifierKeys.Control),
        });

        public static RoutedUICommand OpenSunReturnCMD = new(
        "Open Sun Return",
        nameof(OpenSunReturnCMD),
        typeof(Window),
        new()
        {
            new KeyGesture(Key.R, ModifierKeys.Control),
        });

        public static RoutedUICommand OpenMoonReturnCMD = new(
        "Open Moon Return",
        nameof(OpenMoonReturnCMD),
        typeof(Window),
        new()
        {
           new KeyGesture(Key.M, ModifierKeys.Control),
        });

        public static RoutedUICommand OpenHoroscopeCMD = new(
       "Open Horoscope",
        nameof(OpenHoroscopeCMD),
        typeof(Window),
        new ()
        {
           new KeyGesture(Key.H, ModifierKeys.Control)
        });
        
        public static RoutedUICommand OpenEditCMD = new(
       "Open Edit Form",
        nameof(OpenEditCMD),
        typeof(Window),
        new()
        {
           new KeyGesture(Key.E, ModifierKeys.Control)
        });

        public static RoutedUICommand OpenLocationDownloaderCMD = new(
        "Open Location Downloader Form",
        nameof(OpenLocationDownloaderCMD),
        typeof(Window),
        new()
        {
            new KeyGesture(Key.D, ModifierKeys.Control)
        });

        private SkyEvent _sky = null!;
        public SkyEvent Sky
        {
            get => _sky;
            set 
            { 
                _sky = value;
                NatalChartCanvas.Sky = value;
            }
        }

        private bool _sinastryView = false;
        public ChartViewContainer()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ChartViewContainer(bool isSinastryView = false) : this()
        {
            _sinastryView = isSinastryView;
        }

        private void CollapseUpperRows() 
        {
            if (!_sinastryView) 
            {
                SetWindowBindings();
                return;
            }
            Row1.Height = new(0);
            Row2.Height = new(0);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CollapseUpperRows();
            Window? win = Helper.GetActiveWindow();
            if (win != null)
                win.Closing += WinOnClosing;
        }

        private void SetWindowBindings() 
        {
            Window? activeWin = Helper.GetActiveWindow();
            activeWin?.CommandBindings.Add(new(ApplicationCommands.New, AddNew));
            activeWin?.CommandBindings.Add(new(OpenArchiveCMD, OpenList));
            activeWin?.CommandBindings.Add(new(OpenHoroscopeCMD, OnHoroscopeClicked));
            activeWin?.CommandBindings.Add(new(OpenLocationDownloaderCMD, LocationDownloaderClicked));
            activeWin?.CommandBindings.Add(new(OpenEditCMD, EditClicked));
            activeWin?.CommandBindings.Add(new(ApplicationCommands.Save, SaveClicked));
            activeWin?.CommandBindings.Add(new(OpenSunReturnCMD, SunReturnClick));
            activeWin?.CommandBindings.Add(new(OpenMoonReturnCMD, OpenMoonReturn));
            activeWin?.CommandBindings.Add(new(OpenSinastryCMD, OpenSinastry));
        }

        private void WinOnClosing(object? sender, CancelEventArgs e)
        {
            bool? isDirty = Sky?.Person?.IsDirty;

            if (!isDirty.HasValue) return;
            if (!isDirty.Value) return;

            DialogResult result = UnsavedDialog.Ask("Do you want to save your changes before closing?");
            if (result == DialogResult.No)
            {
                Sky?.Person?.Undo();
                e.Cancel = false; // Allow closing the window.
            }
            else
            {
                if (Sky == null) return;
                IAbstractFormController? controller = Sky.PersonController;
                if (controller == null) return;
                controller.SetCurrentRecord(Sky?.Person);
                bool updateResult = controller.PerformUpdate();
                e.Cancel = !updateResult;
            }

            if (!e.Cancel) 
            {
                Loaded -= OnLoaded;
                Window? win = Helper.GetActiveWindow();
                if (win != null)
                    win.Closing -= WinOnClosing;
            }
        }
        
        private void OpenList(object sender, RoutedEventArgs e) => new PersonListForm().ShowDialog();

        private void AddNew(object sender, RoutedEventArgs e) =>
        new PersonForm(new Person(), false, true) { Owner = Helper.GetActiveWindow() }.ShowDialog();

        private void EditClicked(object sender, RoutedEventArgs e)
        {
            Person? person = Sky.Person;
           
            if (person == null) return;

            PersonForm personForm = new(person, person.IsNewRecord())
            {
                Owner = Helper.GetActiveWindow()
            };

            personForm.ShowDialog();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            IAbstractFormController? controller = Sky.PersonController;
            controller?.SetCurrentRecord(Sky.Person);
            bool? result = controller?.PerformUpdate();

            if (result.HasValue && result.Value) 
            {
                SuccessDialog.Display();
            }
        }

        private void LocationDownloaderClicked(object sender, RoutedEventArgs e)
        {
            new AtlasDownloadForm().ShowDialog();
        }

        private void OnHoroscopeClicked(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win.Title.ToLower().Contains("horoscope"))
            {
                Failure.Allert("Cannot calculate a Horoscope on a Horoscope","Action Denied");
                return;
            }
            new HoroscopeWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenSinastry(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win.Title.ToLower().Contains("return"))
            {
                Failure.Allert("Cannot calculate a Sinastry on a Return", "Action Denied");
                return;
            }

            if (win.Title.ToLower().Contains("horoscope"))
            {
                Failure.Allert("Cannot calculate a Sinastry on a Horoscope", "Action Denied");
                return;
            }

            new PersonListForm() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void SunReturnClick(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win.Title.ToLower().Contains("return"))
            {
                Failure.Allert("Cannot calculate a Return on a Return", "Action Denied");
                return;
            }

            if (win.Title.ToLower().Contains("horoscope"))
            {
                Failure.Allert("Cannot calculate a Return on a Horoscope", "Action Denied");
                return;
            }

            new SunReturnWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenMoonReturn(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win.Title.ToLower().Contains("return"))
            {
                Failure.Allert("Cannot calculate a Return on a Return", "Action Denied");
                return;
            }

            if (win.Title.ToLower().Contains("horoscope"))
            {
                Failure.Allert("Cannot calculate a Return on a Horoscope", "Action Denied");
                return;
            }

            new MoonReturnWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenStarsClicked(object sender, RoutedEventArgs e)
        {
            new PointListWindow(true).ShowDialog();
        }

        private void OpenHouseClicked(object sender, RoutedEventArgs e)
        {
            new PointListWindow(false).ShowDialog();
        }

        private void OpenAspectClicked(object sender, RoutedEventArgs e)
        {
            new AspectListWindow().ShowDialog();
        }

        private void OpenDefaultLocationSettings(object sender, RoutedEventArgs e)
        {
            new SetDefaultCityWindow().ShowDialog();
        }

        private void OpenSignList(object sender, RoutedEventArgs e)
        {
            new SignListWindow().ShowDialog();
        }

        private void OnRadixAspectsClicked(object sender, RoutedEventArgs e)
        {
            new LibraryAspectsWindowList(new TransitType(1)).ShowDialog();    
        }

        private void OnTransitAspectsClicked(object sender, RoutedEventArgs e)
        {
            new LibraryAspectsWindowList(new TransitType(2)).ShowDialog();
        }

        private void OnRadixHousesClicked(object sender, RoutedEventArgs e)
        {
            new LibraryHousesWindowList(new TransitType(1)).ShowDialog();
        }

        private void OnTransitHousesClicked(object sender, RoutedEventArgs e)
        {
            new LibraryHousesWindowList(new TransitType(2)).ShowDialog();
        }

        private void OnReturnHousesClicked(object sender, RoutedEventArgs e)
        {
            new LibraryHousesWindowList(new TransitType(4)).ShowDialog();
        }

        private void OnSinastryHousesClicked(object sender, RoutedEventArgs e)
        {
            new LibraryHousesWindowList(new TransitType(3)).ShowDialog();
        }

        private void OnSinastryAspectsClicked(object sender, RoutedEventArgs e)
        {
            new LibraryAspectsWindowList(new TransitType(3)).ShowDialog();
        }

        private void OnStarInSignClicked(object sender, RoutedEventArgs e)
        {
            new LibrarySignsWindowList().ShowDialog();
        }
    }
}