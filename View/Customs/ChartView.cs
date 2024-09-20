using MvvmHelpers;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.model;

namespace WpfApp1.View
{
    public class ChartView : Control
    {
        public static readonly DependencyProperty GendersProperty =
        DependencyProperty.Register(nameof(Genders), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Genders
        {
            get => (List<FeaturesCount>)GetValue(GendersProperty);
            set => SetValue(GendersProperty, value);
        }

        public static readonly DependencyProperty HousesCountProperty =
        DependencyProperty.Register(nameof(HousesCount), typeof(IEnumerable<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public IEnumerable<FeaturesCount> HousesCount
        {
            get => (IEnumerable<FeaturesCount>)GetValue(HousesCountProperty);
            set => SetValue(HousesCountProperty, value);
        }

        public static readonly DependencyProperty TriplicitiesProperty =
        DependencyProperty.Register(nameof(Triplicities), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Triplicities
        {
            get => (List<FeaturesCount>)GetValue(TriplicitiesProperty);
            set => SetValue(TriplicitiesProperty, value);
        }

        public static readonly DependencyProperty ElementsProperty =
        DependencyProperty.Register(nameof(Elements), typeof(List<FeaturesCount>),
        typeof(ChartView), new PropertyMetadata(null));

        public List<FeaturesCount> Elements
        {
            get => (List<FeaturesCount>)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }

        public static readonly DependencyProperty SkyProperty =
        DependencyProperty.Register(nameof(Sky), typeof(SkyEvent),
        typeof(ChartView), new PropertyMetadata(OnSkyChanged) { DefaultValue = null});

        public SkyEvent Sky
        {
            get => (SkyEvent)GetValue(SkyProperty);
            set => SetValue(SkyProperty, value);
        }

        public ObservableRangeCollection<Star> Stars
        {
            get => (ObservableRangeCollection<Star>)GetValue(StarsProperty);
            set => SetValue(StarsProperty, value);
        }

        public static readonly DependencyProperty AspectsProperty =
            DependencyProperty.Register(
                nameof(Aspects),
                typeof(ObservableRangeCollection<Aspect>),
                typeof(ChartView),
                new PropertyMetadata(null));

        public ObservableRangeCollection<Aspect> Aspects
        {
            get => (ObservableRangeCollection<Aspect>)GetValue(AspectsProperty);
            set => SetValue(AspectsProperty, value);
        }

        public static readonly DependencyProperty StarsProperty =
            DependencyProperty.Register(
                nameof(Stars),
                typeof(ObservableRangeCollection<Star>),
                typeof(ChartView),
                new PropertyMetadata(new ObservableRangeCollection<Star>()));

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