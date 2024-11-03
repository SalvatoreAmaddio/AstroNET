using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AstroNET.model;
using System.ComponentModel;
using System.IO;
using Backend.Utils;
using AstroNETLibrary.Sky;
using AstroNET.View.Library;

namespace AstroNET.View
{
    public partial class ChartViewContainer : Grid
    {
        private bool _sinastryView = false;
        private AbstractSkyEvent _sky = null!;
        private bool IsHoroscope => _sky is SkyEvent sky && sky.Horoscope != null;

        #region RoutedUICommand
        public static RoutedUICommand OpenStarsCMD = Helper.CreateRoutedUICMD(
        "Open Stars", nameof(OpenStarsCMD), Key.P);

        public static RoutedUICommand OpenArchiveCMD = Helper.CreateRoutedUICMD(
        "Open Archive", nameof(OpenArchiveCMD), Key.A);

        public static RoutedUICommand OpenSinastryCMD = Helper.CreateRoutedUICMD(
        "Open Sinastry", nameof(OpenSinastryCMD), Key.C);

        public static RoutedUICommand OpenSunReturnCMD = Helper.CreateRoutedUICMD(
        "Open Sun Return", nameof(OpenSunReturnCMD), Key.R);

        public static RoutedUICommand OpenMoonReturnCMD = Helper.CreateRoutedUICMD(
        "Open Moon Return", nameof(OpenMoonReturnCMD), Key.M);

        public static RoutedUICommand OpenHoroscopeCMD = Helper.CreateRoutedUICMD(
       "Open Horoscope", nameof(OpenHoroscopeCMD), Key.H);

        public static RoutedUICommand OpenLocationDownloaderCMD = Helper.CreateRoutedUICMD(
        "Open Location Downloader Form", nameof(OpenLocationDownloaderCMD), Key.D);

        public static RoutedUICommand OpenTransitCMD = Helper.CreateRoutedUICMD(
        "Open Transit Form", nameof(OpenTransitCMD), Key.T);
        #endregion

        public ICommand OpenCurtainCMD { get; }
        public AbstractSkyEvent Sky
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
            Curtain.SoftwareInfo = new SoftwareInfo("Salvatore Amaddio", "www.salvatoreamaddio.co.uk", "Mister J", "2024");
            DataContext = this;
            Loaded += OnLoaded;
            OpenCurtainCMD = new CMD(OpenCurtain);
        }

        public ChartViewContainer(bool isSinastryView = false) : this()
        {
            _sinastryView = isSinastryView;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetWindowBindings();
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
            activeWin?.CommandBindings.Add(new(OpenSunReturnCMD, OpenSunReturn));
            activeWin?.CommandBindings.Add(new(OpenMoonReturnCMD, OpenMoonReturn));
            activeWin?.CommandBindings.Add(new(OpenSinastryCMD, OpenSinastry));
            activeWin?.CommandBindings.Add(new(OpenStarsCMD, OpenStars));
            activeWin?.CommandBindings.Add(new(OpenTransitCMD, OpenTransitCalcualtor));
        }

        private void WinOnClosing(object? sender, CancelEventArgs e)
        {
            bool? isDirty = ((Person?)Sky?.Person)?.IsDirty;

            if (!isDirty.HasValue) return;
            if (!isDirty.Value) return;

            DialogResult result = UnsavedDialog.Ask("Do you want to save your changes before closing?");
            if (result == DialogResult.No)
            {
                ((Person?)Sky?.Person)?.Undo();
                e.Cancel = false; // Allow closing the window.
            }
            else
            {
                if (Sky == null) return;
                IAbstractFormController? controller = (IAbstractFormController?)Sky.PersonController;
                if (controller == null) return;
                controller.SetCurrentRecord((Person?)Sky?.Person);
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

        private void OpenCurtain() => Curtain.Open();
        private void OpenArchive(object sender, RoutedEventArgs e) => new PersonListForm().ShowDialog();

        private void AddNew(object sender, RoutedEventArgs e) =>
        new PersonForm(new Person(), false, true) { Owner = Helper.GetActiveWindow() }.ShowDialog();

        private void OpenLocationDownloader(object sender, RoutedEventArgs e) =>
        new AtlasDownloadForm().ShowDialog();

        private void OpenHoroscope(object sender, RoutedEventArgs e)
        {
            if (((Person)Sky!.Person!).IsNewRecord())
            {
                Failure.Allert("Cannot calculate Horoscope on an unsaved Sky");
                return;
            }
            new HoroscopeWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenSinastry(object sender, RoutedEventArgs e)
        {
            if (((Person)Sky!.Person!).IsNewRecord())
            {
                Failure.Allert("Cannot calculate Sinastry on an unsaved Sky");
                return;
            }

            new PersonListForm() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenSunReturn(object sender, RoutedEventArgs e)
        {
            if (((Person)Sky!.Person!).IsNewRecord())
            {
                Failure.Allert("Cannot calculate Returns on an unsaved Sky");
                return;
            }
            new SunReturnWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OpenMoonReturn(object sender, RoutedEventArgs e)
        {
            if (((Person)Sky!.Person!).IsNewRecord())
            {
                Failure.Allert("Cannot calculate Returns on an unsaved Sky");
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
        new LibraryStarSignsWindowList().ShowDialog();

        private void OnHouseInSignClicked(object sender, RoutedEventArgs e) =>
        new LibraryHouseSignsWindowList().ShowDialog();

        private static void OpenLibraryHouses(int id) => new LibraryStarHousesWindowList(new TransitType(id)).ShowDialog();
        private static void OpenLibraryAspects(int id) => new LibraryStarAspectsWindowList(new TransitType(id)).ShowDialog();

        private void OpenTransitCalcualtor(object sender, RoutedEventArgs e) 
        {
            if (((Person)Sky!.Person!).IsNewRecord())
            {
                Failure.Allert("Cannot calculate Transits on an unsaved Sky");
                return;
            }
            new TransitCalculatorWindow() { Owner = Helper.GetActiveWindow() }.ShowDialog();
        }

        private void OnExportClicked(object sender, RoutedEventArgs e)
        {
            string? winTitle = Helper.GetActiveWindow()?.Title;

            NatalChartCanvas.Chart.Screenshot(Path.Combine(Sys.Desktop, $"{winTitle}_{Sky.SkyInfo.Day}_{Sky.SkyInfo.Month}_{Sky.SkyInfo.Year}.png"));
        }

        private void OnReferencesClick(object sender, RoutedEventArgs e) =>
        new References().ShowDialog();

        private void OnNatalChartRulesClicked(object sender, RoutedEventArgs e) =>
        new NatalChartRules().Show();

        private void OnSinastryRulesClicked(object sender, RoutedEventArgs e) =>
        new SinastryRules().Show();

    }
}