﻿using MvvmHelpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.model;

namespace WpfApp1.View
{
    public class ChartView : Control
    {
        #region Genders
        public static readonly DependencyProperty GendersProperty =
        DependencyProperty.Register(nameof(Genders), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Genders
        {
            get => (List<FeaturesCount>)GetValue(GendersProperty);
            set => SetValue(GendersProperty, value);
        }
        #endregion

        #region HousesCount
        public static readonly DependencyProperty HousesCountProperty =
        DependencyProperty.Register(nameof(HousesCount), typeof(IEnumerable<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public IEnumerable<FeaturesCount> HousesCount
        {
            get => (IEnumerable<FeaturesCount>)GetValue(HousesCountProperty);
            set => SetValue(HousesCountProperty, value);
        }
        #endregion

        #region Triplicities
        public static readonly DependencyProperty TriplicitiesProperty =
        DependencyProperty.Register(nameof(Triplicities), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Triplicities
        {
            get => (List<FeaturesCount>)GetValue(TriplicitiesProperty);
            set => SetValue(TriplicitiesProperty, value);
        }
        #endregion

        #region Elements
        public static readonly DependencyProperty ElementsProperty =
        DependencyProperty.Register(nameof(Elements), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Elements
        {
            get => (List<FeaturesCount>)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }
        #endregion

        #region Sky
        public static readonly DependencyProperty SkyProperty =
        DependencyProperty.Register(nameof(Sky), typeof(SkyEvent),
        typeof(ChartView), new PropertyMetadata(OnSkyChanged) { DefaultValue = null});

        public SkyEvent Sky
        {
            get => (SkyEvent)GetValue(SkyProperty);
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
            Interpretation interpretation = new(LibrarySearch.SearchAspect((Aspect)e.NewValue));
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
                new PropertyMetadata(OnSelectedStarChanged));

        private static void OnSelectedStarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Interpretation interpretation;

            if (e.NewValue is Star)
                interpretation = new(LibrarySearch.SearchStar((Star)e.NewValue));
            else
                interpretation = new(LibrarySearch.SearchHouse((House)e.NewValue));

            interpretation.Show();
        }

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
                new PropertyMetadata(OnSelectedStarChanged));

        public House? SelectedHouse
        {
            get => (House?)GetValue(SelectedHouseProperty);
            set => SetValue(SelectedHouseProperty, value);
        }
        #endregion

        #region Aspects
        public ObservableRangeCollection<Aspect> Aspects
        {
            get => (ObservableRangeCollection<Aspect>)GetValue(AspectsProperty);
            set => SetValue(AspectsProperty, value);
        }

        public static readonly DependencyProperty AspectsProperty =
            DependencyProperty.Register(
                nameof(Aspects),
                typeof(ObservableRangeCollection<Aspect>),
                typeof(ChartView),
                new PropertyMetadata(null));
        #endregion

        #region Stars
        public ObservableRangeCollection<Star> Stars
        {
            get => (ObservableRangeCollection<Star>)GetValue(StarsProperty);
            set => SetValue(StarsProperty, value);
        }

        public static readonly DependencyProperty StarsProperty =
            DependencyProperty.Register(
                nameof(Stars),
                typeof(ObservableRangeCollection<Star>),
                typeof(ChartView),
                new PropertyMetadata(new ObservableRangeCollection<Star>()));
        #endregion

        #region Houses
        public ObservableRangeCollection<House> Houses
        {
            get => (ObservableRangeCollection<House>)GetValue(HousesProperty);
            set => SetValue(HousesProperty, value);
        }

        public static readonly DependencyProperty HousesProperty =
            DependencyProperty.Register(
                nameof(Houses),
                typeof(ObservableRangeCollection<House>),
                typeof(ChartView),
                new PropertyMetadata(new ObservableRangeCollection<House>()));
        #endregion

        private static void OnSkyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartView? control = d as ChartView;
            control?.SetBinding();
        }

        static ChartView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartView), new FrameworkPropertyMetadata(typeof(ChartView)));
        }

        public ChartView() 
        {
        }

        private void SetBinding()
        {
            SkyEvent toUse = (Sky.Horoscope == null) ? Sky : Sky.Horoscope;

            Aspects = toUse.RadixAspects;
            Stars = toUse.Stars;
            Houses = toUse.Houses;

            Triplicities = [];
            Elements = [];
            Genders = [];

            HousesCount = toUse.HousesCount;

            Triplicities.Add(new("Fixed Signs", toUse.FixedSigns));
            Triplicities.Add(new("Cardinal Signs", toUse.FixedSigns));
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