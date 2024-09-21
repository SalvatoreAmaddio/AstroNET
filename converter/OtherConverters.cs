using Backend.Database;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WpfApp1.model;
using System;
using FrontEnd.Model;

namespace WpfApp1.converter
{
    public class TogleBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    abstract public class GetRecordByID<M> : IValueConverter where M : IAbstractModel, new()
    {
        private static IEnumerable<M>? Records = DatabaseManager.Find<M>()?.MasterSource.Cast<M>();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Records.FirstOrDefault(s=>(Int64?)s.GetPrimaryKey().GetValue() == (Int64)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetElement : GetRecordByID<Element> { }
    public class GetTriplicity : GetRecordByID<Triplicity> { }
    public class GetGender : GetRecordByID<Gender> { }
    public class PowerConverters : IValueConverter
    {
        private static IEnumerable<Sign>? Signs = DatabaseManager.Find<Sign>()?.MasterSource.Cast<Sign>();
        private static IEnumerable<StarPower>? Powers = DatabaseManager.Find<StarPower>()?.MasterSource.Cast<StarPower>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new();
            Star star = (Star)value;

            var results = Powers?.Where(s => s.Star.PointId == star.PointId && s.Power.PowerID == System.Convert.ToInt64(parameter)).ToList();
            if (results == null || results.Count == 0) return "?";

            foreach (var result in results)
            {
                Sign? sign = Signs?.FirstOrDefault(s => s.Equals(result.Sign));
                sb.Append((sign==null) ? "?" : sign?.SignName);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyConverter : GetRecordByID<Energy>
    {

    }
}
