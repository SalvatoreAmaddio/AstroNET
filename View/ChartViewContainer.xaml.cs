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
        public static RoutedUICommand OpenStarsCMD = CreateUICMD(
        "Open Stars", nameof(OpenStarsCMD), Key.P);

        public static RoutedUICommand OpenArchiveCMD = CreateUICMD(
        "Open Archive", nameof(OpenArchiveCMD), Key.A);

        public static RoutedUICommand OpenSinastryCMD = CreateUICMD(
        "Open Sinastry", nameof(OpenSinastryCMD), Key.C);

        public static RoutedUICommand OpenSunReturnCMD = CreateUICMD(
        "Open Sun Return", nameof(OpenSunReturnCMD), Key.R);

        public static RoutedUICommand OpenMoonReturnCMD = CreateUICMD(
        "Open Moon Return", nameof(OpenMoonReturnCMD), Key.M);

        public static RoutedUICommand OpenHoroscopeCMD = CreateUICMD(
       "Open Horoscope", nameof(OpenHoroscopeCMD), Key.H);

        public static RoutedUICommand OpenEditCMD = CreateUICMD(
       "Open Edit Form", nameof(OpenEditCMD), Key.E);

        public static RoutedUICommand OpenLocationDownloaderCMD = CreateUICMD(
        "Open Location Downloader Form", nameof(OpenLocationDownloaderCMD), Key.D);

        private static RoutedUICommand CreateUICMD(string text, string name, Key key) => 
        new(text, name, typeof(Window), [new KeyGesture(key, ModifierKeys.Control)]);
        
        private bool _sinastryView = false;
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
            activeWin?.CommandBindings.Add(new(OpenArchiveCMD, OpenArchive));
            activeWin?.CommandBindings.Add(new(OpenHoroscopeCMD, OpenHoroscope));
            activeWin?.CommandBindings.Add(new(OpenLocationDownloaderCMD, OpenLocationDownloader));
            activeWin?.CommandBindings.Add(new(OpenEditCMD, OpenEdit));
            activeWin?.CommandBindings.Add(new(ApplicationCommands.Save, SaveClicked));
            activeWin?.CommandBindings.Add(new(OpenSunReturnCMD, OpenSunReturn));
            activeWin?.CommandBindings.Add(new(OpenMoonReturnCMD, OpenMoonReturn));
            activeWin?.CommandBindings.Add(new(OpenSinastryCMD, OpenSinastry));
            activeWin?.CommandBindings.Add(new(OpenStarsCMD, OpenStars));
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
        
        private void OpenArchive(object sender, RoutedEventArgs e) => new PersonListForm().ShowDialog();

        private void AddNew(object sender, RoutedEventArgs e) =>
        new PersonForm(new Person(), false, true) { Owner = Helper.GetActiveWindow() }.ShowDialog();

        private void OpenEdit(object sender, RoutedEventArgs e)
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

        private void OpenLocationDownloader(object sender, RoutedEventArgs e) =>
        new AtlasDownloadForm().ShowDialog();
        
        private void OpenHoroscope(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win!.Title.ToLower().Contains("horoscope"))
            {
                Failure.Allert("Cannot calculate a Horoscope on a Horoscope","Action Denied");
                return;
            }
            new HoroscopeWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenSinastry(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win!.Title.ToLower().Contains("return"))
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

        private void OpenSunReturn(object sender, RoutedEventArgs e)
        {
            Window? win = Helper.GetActiveWindow();

            if (win!.Title.ToLower().Contains("return"))
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

            if (win!.Title.ToLower().Contains("return"))
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

        private void OpenStars(object sender, RoutedEventArgs e) =>
        new PointListWindow(true).ShowDialog();

        private void OpenHouses(object sender, RoutedEventArgs e) =>
        new PointListWindow(false).ShowDialog();

        private void OpenAspectClicked(object sender, RoutedEventArgs e) =>
        new AspectListWindow().ShowDialog();

        private void OpenDefaultLocationSettings(object sender, RoutedEventArgs e) =>
        new SetDefaultCityWindow().ShowDialog();

        private void OpenSignList(object sender, RoutedEventArgs e) =>
        new SignListWindow().ShowDialog();

        private void OnRadixAspectsClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryAspects(1);    

        private void OnTransitAspectsClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryAspects(2);

        private void OnSinastryAspectsClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryAspects(3);

        private void OnRadixHousesClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryHouses(1);

        private void OnTransitHousesClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryHouses(2);

        private void OnReturnHousesClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryHouses(4);

        private void OnSinastryHousesClicked(object sender, RoutedEventArgs e) =>
        OpenLibraryHouses(3);

        private void OnStarInSignClicked(object sender, RoutedEventArgs e) =>
        new LibrarySignsWindowList().ShowDialog();

        private static void OpenLibraryHouses(int id)=> new LibraryHousesWindowList(new TransitType(id)).ShowDialog();
        private static void OpenLibraryAspects(int id) => new LibraryAspectsWindowList(new TransitType(id)).ShowDialog();

        private void OpenTransitCalcualtor(object sender, RoutedEventArgs e)
        {
            new TransitCalculatorWindow().ShowDialog();
        }
    }
}