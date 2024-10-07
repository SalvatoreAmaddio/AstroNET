using Backend.Database;
using MvvmHelpers;
using System.Windows;
using System.Windows.Controls;
using AstroNET.model;
using FrontEnd.Utils;
using System.Windows.Input;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using AstroNET.controller;
using FrontEnd.Model;

namespace AstroNET.View
{
    public class ChartView : Control
    {
        #region Genders
        public static readonly DependencyProperty GendersProperty =
        DependencyProperty.Register(nameof(Genders), typeof(List<ElementGroupKey>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<ElementGroupKey> Genders
        {
            get => (List<ElementGroupKey>)GetValue(GendersProperty);
            set => SetValue(GendersProperty, value);
        }
        #endregion

        #region OccupiedHouses
        public static readonly DependencyProperty OccupiedHousesProperty =
        DependencyProperty.Register(nameof(OccupiedHouses), typeof(IEnumerable<ElementGroupKey>),
        typeof(ChartView), new PropertyMetadata(null));

        public IEnumerable<ElementGroupKey>? OccupiedHouses
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(OccupiedHousesProperty);
            set => SetValue(OccupiedHousesProperty, value);
        }
        #endregion

        #region Stelliums
        public static readonly DependencyProperty StelliumsProperty =
        DependencyProperty.Register(nameof(Stelliums), typeof(IEnumerable<ElementGroupKey>),
        typeof(ChartView), new PropertyMetadata(null));

        public IEnumerable<ElementGroupKey>? Stelliums
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(StelliumsProperty);
            set => SetValue(StelliumsProperty, value);
        }
        #endregion

        #region SelectedStellium
        public static readonly DependencyProperty SelectedStelliumProperty =
        DependencyProperty.Register(nameof(SelectedStellium), typeof(ElementGroupKey),
        typeof(ChartView), new PropertyMetadata(OnSelectedStelliumChanged));

        private static void OnSelectedStelliumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int skyTypeId = ((ChartView)(d)).Sky.SkyInfo.SkyTypeId;

            if (e.NewValue != null && (skyTypeId == 4 || skyTypeId == 5))
            {
                House? house = DatabaseManager.Find<House>()?.MasterSource.Cast<House>()?.FirstOrDefault(s => s.PointName.Trim().Equals(((ElementGroupKey)e.NewValue).Name.ToString()?.Trim()));
                if (house == null) return;
                new Interpretation(LibrarySearch.SearchStarDescription(new(house), 4)).Show();
            }
        }

        public ElementGroupKey SelectedStellium
        {
            get => (ElementGroupKey)GetValue(SelectedStelliumProperty);
            set => SetValue(SelectedStelliumProperty, value);
        }
        #endregion

        #region Triplicities
        public static readonly DependencyProperty TriplicitiesProperty =
        DependencyProperty.Register(nameof(Triplicities), typeof(List<ElementGroupKey>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<ElementGroupKey> Triplicities
        {
            get => (List<ElementGroupKey>)GetValue(TriplicitiesProperty);
            set => SetValue(TriplicitiesProperty, value);
        }
        #endregion

        #region Elements
        public static readonly DependencyProperty ElementsProperty =
        DependencyProperty.Register(nameof(Elements), typeof(List<ElementGroupKey>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<ElementGroupKey> Elements
        {
            get => (List<ElementGroupKey>)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }
        #endregion

        #region Sky
        public static readonly DependencyProperty SkyProperty =
        DependencyProperty.Register(nameof(Sky), typeof(AbstractSkyEvent),
        typeof(ChartView), new PropertyMetadata(OnSkyChanged) { DefaultValue = null });

        public AbstractSkyEvent Sky
        {
            get => (AbstractSkyEvent)GetValue(SkyProperty);
            set => SetValue(SkyProperty, value);
        }
        #endregion

        #region SelectedAspect
        public static readonly DependencyProperty SelectedAspectProperty =
            DependencyProperty.Register(
                nameof(SelectedAspect),
                typeof(Aspect),
                typeof(ChartView),
                new PropertyMetadata(OnSelectedAspectChanged));

        private static void OnSelectedAspectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            Interpretation interpretation;
            Aspect aspect = (Aspect)e.NewValue;
            ChartView control = (ChartView)d;

            if (control.Sky is ReturnSkyEvent returnSky)
            {
                interpretation = new(LibrarySearch.SearchAspect(aspect, returnSky.HouseHostingReturnAsc), aspect);
            }
            else
            {
                interpretation = new(LibrarySearch.SearchAspect(aspect), aspect);
            }
            interpretation.Show();
        }

        public Aspect? SelectedAspect
        {
            get => (Aspect?)GetValue(SelectedAspectProperty);
            set => SetValue(SelectedAspectProperty, value);
        }
        #endregion

        #region SelectedStar
        public static readonly DependencyProperty SelectedStarProperty =
            DependencyProperty.Register(
                nameof(SelectedStar),
                typeof(Star),
                typeof(ChartView),
                new PropertyMetadata(OnSelectedPointChanged));
        public Star? SelectedStar
        {
            get => (Star?)GetValue(SelectedStarProperty);
            set => SetValue(SelectedStarProperty, value);
        }
        #endregion

        #region SelectedHouse
        public static readonly DependencyProperty SelectedHouseProperty =
            DependencyProperty.Register(
                nameof(SelectedHouse),
                typeof(House),
                typeof(ChartView),
                new PropertyMetadata(OnSelectedPointChanged));

        public House? SelectedHouse
        {
            get => (House?)GetValue(SelectedHouseProperty);
            set => SetValue(SelectedHouseProperty, value);
        }
        #endregion

        #region Aspects
        public ObservableRangeCollection<IAspect> Aspects
        {
            get => (ObservableRangeCollection<IAspect>)GetValue(AspectsProperty);
            set => SetValue(AspectsProperty, value);
        }

        public static readonly DependencyProperty AspectsProperty =
            DependencyProperty.Register(
                nameof(Aspects),
                typeof(ObservableRangeCollection<IAspect>),
                typeof(ChartView),
                new PropertyMetadata(null));
        #endregion

        #region Stars
        public ObservableRangeCollection<IStar> Stars
        {
            get => (ObservableRangeCollection<IStar>)GetValue(StarsProperty);
            set => SetValue(StarsProperty, value);
        }

        public static readonly DependencyProperty StarsProperty =
            DependencyProperty.Register(
                nameof(Stars),
                typeof(ObservableRangeCollection<IStar>),
                typeof(ChartView),
                new PropertyMetadata(new ObservableRangeCollection<IStar>()));
        #endregion

        #region Houses
        public ObservableRangeCollection<IHouse> Houses
        {
            get => (ObservableRangeCollection<IHouse>)GetValue(HousesProperty);
            set => SetValue(HousesProperty, value);
        }

        public static readonly DependencyProperty HousesProperty =
            DependencyProperty.Register(
                nameof(Houses),
                typeof(ObservableRangeCollection<IHouse>),
                typeof(ChartView),
                new PropertyMetadata(new ObservableRangeCollection<IHouse>()));
        #endregion

        #region OpenEditCMD
        public ICommand OpenEditCMD
        {
            get => (ICommand)GetValue(OpenEditCMDProperty);
            set => SetValue(OpenEditCMDProperty, value);
        }

        public static readonly DependencyProperty OpenEditCMDProperty =
            DependencyProperty.Register(
                nameof(OpenEditCMD),
                typeof(ICommand),
                typeof(ChartView),
                new PropertyMetadata());
        #endregion

        #region SaveCMD
        public ICommand SaveCMD
        {
            get => (ICommand)GetValue(SaveCMDProperty);
            set => SetValue(SaveCMDProperty, value);
        }

        public static readonly DependencyProperty SaveCMDProperty =
            DependencyProperty.Register(
                nameof(SaveCMD),
                typeof(ICommand),
                typeof(ChartView),
                new PropertyMetadata());
        #endregion

        #region ToolBarHeight
        public double ToolBarHeight
        {
            get => (double)GetValue(ToolBarHeightProperty);
            set => SetValue(ToolBarHeightProperty, value);
        }

        public static readonly DependencyProperty ToolBarHeightProperty =
            DependencyProperty.Register(
                nameof(ToolBarHeight),
                typeof(double),
                typeof(ChartView),
                new PropertyMetadata(25.0));
        #endregion

        private static void OnSelectedPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Interpretation interpretation;
            ChartView control = (ChartView)d;

            int skyTypeId = control.Sky.SkyInfo.SkyTypeId;

            if (control.Sky is SkyEvent sky && sky.Horoscope != null)
            {
                skyTypeId = 2;
            }

            if (e.NewValue is Star star)
                interpretation = new(LibrarySearch.SearchStarDescription(star, skyTypeId), e.NewValue, skyTypeId);
            else
            {
                if (skyTypeId == 4 || skyTypeId == 5) return;
                interpretation = new(LibrarySearch.SearchHouseSignDescription((House)e.NewValue), e.NewValue);
            }

            interpretation.Show();
        }
        private static void OnSkyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartView? control = d as ChartView;
            control?.SetBinding();
        }

        public ZodiacChart Chart { get; private set; } = null!;

        public static RoutedUICommand OpenEditUICMD = Helper.CreateRoutedUICMD(
        "Open Edit Form", nameof(OpenEditCMD), Key.E);

        #region EditButtonWidth
        public double EditButtonWidth
        {
            get => (double)GetValue(EditButtonWidthProperty);
            set => SetValue(EditButtonWidthProperty, value);
        }

        public static readonly DependencyProperty EditButtonWidthProperty =
            DependencyProperty.Register(
                nameof(EditButtonWidth),
                typeof(double),
                typeof(ChartView),
                new PropertyMetadata(30.0));
        #endregion

        private bool _isHoroscope = false;

        public bool IsHoroscope
        {
            get => _isHoroscope;
            set
            {
                _isHoroscope = value;
                EditButtonWidth = value ? 0 : 20;
            }
        }

        static ChartView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartView), new FrameworkPropertyMetadata(typeof(ChartView)));
        }

        public ChartView()
        {
            OpenEditCMD = new CMD(OpenEdit);
            SaveCMD = new CMD(Save);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window? activeWin = Helper.GetActiveWindow();
            activeWin?.CommandBindings.Add(new(OpenEditUICMD, (s, e) => OpenEditCMD.Execute(null)));
            activeWin?.CommandBindings.Add(new(ApplicationCommands.Save, (s, e) => SaveCMD.Execute(null)));
        }

        private void Save()
        {
            if (ToolBarHeight == 0) return;

            IAbstractFormController? controller;
            IAbstractModel record;

            if (Sky is SkyEvent skyEvent)
            {
                if (skyEvent.Horoscope != null)
                {
                    controller = new SavedChartControllerList();
                    record = new SavedCharts(skyEvent.Horoscope);
                    record.IsDirty = true;
                }
                else
                {
                    controller = (IAbstractFormController?)Sky.PersonController;
                    record = (IAbstractModel)Sky.Person;
                }

                controller?.SetCurrentRecord(record);

                bool? result = controller?.PerformUpdate();

                if (result.HasValue && result.Value)
                {
                    SuccessDialog.Display();
                }
            }
        }

        private void OpenEdit()
        {
            if (ToolBarHeight == 0 || IsHoroscope) return;

            Person? person = (Person?)Sky.Person;

            if (person == null) return;

            PersonForm personForm = new(person, person.IsNewRecord())
            {
                Owner = Helper.GetActiveWindow()
            };

            personForm.ShowDialog();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Chart = (ZodiacChart)GetTemplateChild("ZodiacChart");
        }

        private void SetBinding()
        {
            AbstractSkyEvent toUse;

            if (Sky is SkyEvent sky)
            {
                toUse = (sky.Horoscope == null) ? Sky : sky.Horoscope;
            }
            else
            {
                toUse = Sky;
            }

            Aspects = toUse.RadixAspects;
            Stars = toUse.Stars;
            Houses = toUse.Houses;

            Triplicities = [];
            Elements = [];
            Genders = [];

            OccupiedHouses = toUse.OccupiedHouses;
            Stelliums = toUse.Stelliums;

            Triplicities.Add(new("Fixed Signs", toUse.FixedSigns));
            Triplicities.Add(new("Cardinal Signs", toUse.CardinalSigns));
            Triplicities.Add(new("Mobile Signs", toUse.MobileSigns));

            Elements.Add(new("Water", toUse.WaterSigns));
            Elements.Add(new("Fire", toUse.FireSigns));
            Elements.Add(new("Air", toUse.AirSigns));
            Elements.Add(new("Earth", toUse.EarhSigns));

            Genders.Add(new("Female Signs", toUse.FemaleSigns));
            Genders.Add(new("Male Signs", toUse.MaleSigns));
        }
    }
}