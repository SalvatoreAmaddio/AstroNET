using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.model;

namespace WpfApp1.View
{
    public class SinastryChart : Control
    {
        //IEnumerable<ElementGroupKey>? Stelliums1

        #region Chart1StelliumsList
        public static readonly DependencyProperty Chart1StelliumsListProperty =
        DependencyProperty.Register(nameof(Chart1StelliumsList), typeof(IEnumerable<ElementGroupKey>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<ElementGroupKey>? Chart1StelliumsList
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(Chart1StelliumsListProperty);
            set => SetValue(Chart1StelliumsListProperty, value);
        }
        #endregion

        #region Chart2StelliumsList
        public static readonly DependencyProperty Chart2StelliumsListProperty =
        DependencyProperty.Register(nameof(Chart2StelliumsList), typeof(IEnumerable<ElementGroupKey>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<ElementGroupKey>? Chart2StelliumsList
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(Chart2StelliumsListProperty);
            set => SetValue(Chart2StelliumsListProperty, value);
        }
        #endregion

        #region Chart1OccupiedHousesList
        public static readonly DependencyProperty Chart1OccupiedHousesListProperty =
        DependencyProperty.Register(nameof(Chart1OccupiedHousesList), typeof(IEnumerable<ElementGroupKey>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<ElementGroupKey>? Chart1OccupiedHousesList
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(Chart1OccupiedHousesListProperty);
            set => SetValue(Chart1OccupiedHousesListProperty, value);
        }
        #endregion

        #region Chart1OccupiedHousesList
        public static readonly DependencyProperty Chart2OccupiedHousesListProperty =
        DependencyProperty.Register(nameof(Chart2OccupiedHousesList), typeof(IEnumerable<ElementGroupKey>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<ElementGroupKey>? Chart2OccupiedHousesList
        {
            get => (IEnumerable<ElementGroupKey>?)GetValue(Chart2OccupiedHousesListProperty);
            set => SetValue(Chart2OccupiedHousesListProperty, value);
        }
        #endregion

        #region Chart1ZodiacListTitle
        public static readonly DependencyProperty Chart1ZodiacListTitleProperty =
        DependencyProperty.Register(nameof(Chart1ZodiacListTitle), typeof(string),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = string.Empty });

        public string Chart1ZodiacListTitle
        {
            get => (string)GetValue(Chart1ZodiacListTitleProperty);
            set => SetValue(Chart1ZodiacListTitleProperty, value);
        }
        #endregion

        #region Chart1ZodiacList
        public static readonly DependencyProperty Chart1ZodiacListProperty =
        DependencyProperty.Register(nameof(Chart1ZodiacList), typeof(IEnumerable<Star>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<Star>? Chart1ZodiacList
        {
            get => (IEnumerable<Star>?)GetValue(Chart1ZodiacListProperty);
            set => SetValue(Chart1ZodiacListProperty, value);
        }
        #endregion

        #region Chart2ZodiacListTitle
        public static readonly DependencyProperty Chart2ZodiacListTitleProperty =
        DependencyProperty.Register(nameof(Chart2ZodiacListTitle), typeof(string),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = string.Empty });

        public string Chart2ZodiacListTitle
        {
            get => (string)GetValue(Chart2ZodiacListTitleProperty);
            set => SetValue(Chart2ZodiacListTitleProperty, value);
        }
        #endregion

        #region Chart2ZodiacList
        public static readonly DependencyProperty Chart2ZodiacListProperty =
        DependencyProperty.Register(nameof(Chart2ZodiacList), typeof(IEnumerable<Star>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<Star>? Chart2ZodiacList
        {
            get => (IEnumerable<Star>?)GetValue(Chart2ZodiacListProperty);
            set => SetValue(Chart2ZodiacListProperty, value);
        }
        #endregion

        #region Chart1AspectsList
        public static readonly DependencyProperty Chart1AspectsListProperty =
        DependencyProperty.Register(nameof(Chart1AspectsList), typeof(IEnumerable<Aspect>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<Aspect> Chart1AspectsList
        {
            get => (IEnumerable<Aspect>)GetValue(Chart1AspectsListProperty);
            set => SetValue(Chart1AspectsListProperty, value);
        }
        #endregion

        #region Chart2AspectsList
        public static readonly DependencyProperty Chart2AspectsListProperty =
        DependencyProperty.Register(nameof(Chart2AspectsList), typeof(IEnumerable<Aspect>),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public IEnumerable<Aspect> Chart2AspectsList
        {
            get => (IEnumerable<Aspect>)GetValue(Chart2AspectsListProperty);
            set => SetValue(Chart2AspectsListProperty, value);
        }
        #endregion

        #region Person1
        public static readonly DependencyProperty Person1Property =
        DependencyProperty.Register(nameof(Person1), typeof(Person),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public Person Person1
        {
            get => (Person)GetValue(Person1Property);
            set => SetValue(Person1Property, value);
        }
        #endregion

        #region Person2
        public static readonly DependencyProperty Person2Property =
        DependencyProperty.Register(nameof(Person2), typeof(Person),
        typeof(SinastryChart), new PropertyMetadata() { DefaultValue = null });

        public Person Person2
        {
            get => (Person)GetValue(Person2Property);
            set => SetValue(Person2Property, value);
        }
        #endregion

        #region SinastryBundler
        public static readonly DependencyProperty SinastryBundleProperty =
        DependencyProperty.Register(nameof(SinastryBundle), typeof(SinastryBundle),
        typeof(SinastryChart), new PropertyMetadata(OnSinastryBundleChanged) { DefaultValue = null });

        private static void OnSinastryBundleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SinastryChart)d).SetProperties((SinastryBundle?)e.NewValue);            
        }

        public SinastryBundle SinastryBundle
        {
            get => (SinastryBundle)GetValue(SinastryBundleProperty);
            set => SetValue(SinastryBundleProperty, value);
        }
        #endregion

        static SinastryChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinastryChart), new FrameworkPropertyMetadata(typeof(SinastryChart)));
        }

        private void SetProperties(SinastryBundle? sinastryBundle) 
        {
            if (sinastryBundle == null) return;

            Person1 = sinastryBundle.Person1;
            Person2 = sinastryBundle.Person2;
            
            Chart1AspectsList = sinastryBundle.Chart1Aspects;
            Chart2AspectsList = sinastryBundle.Chart2Aspects;

            Chart1ZodiacListTitle = sinastryBundle.Chart1ZodiacTile;
            Chart1ZodiacList = sinastryBundle.Chart1Zodiac;

            Chart2ZodiacListTitle = sinastryBundle.Chart2ZodiacTile;
            Chart2ZodiacList = sinastryBundle.Chart2Zodiac;

            Chart1OccupiedHousesList = sinastryBundle.Chart1OccupiedHouses;
            Chart2OccupiedHousesList = sinastryBundle.Chart2OccupiedHouses;

            Chart1StelliumsList = sinastryBundle.Chart1Stelliums;
            Chart2StelliumsList = sinastryBundle.Chart2Stelliums;
        }
    }
}
