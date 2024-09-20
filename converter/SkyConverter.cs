using Backend.Database;
using Backend.Source;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WpfApp1.model;
using Region = WpfApp1.model.Region;

namespace WpfApp1.converter
{
    public class RetrogradeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRetrograde = (bool)value;
            return (isRetrograde) ? "R" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenderConvert : IValueConverter
    {
        private MasterSource genders => DatabaseManager.Find<Gender>()!.MasterSource;
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return genders.FirstOrDefault(s=>s.Equals(value))?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UnknownTimeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class ImageLoaderConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? path = value as string;
            if (string.IsNullOrEmpty(path)) return null;

            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RegionConverter : IValueConverter
    {
        protected IEnumerable<Region> _regions = DatabaseManager.Find<Region>()!.MasterSource.Cast<Region>();
        public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _regions.FirstOrDefault(s=>s.Equals(value))?.RegionName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Region converter");
        }
    }

    public class CountryFromRegionConverter : RegionConverter
    {
        protected IEnumerable<Country> _countries = DatabaseManager.Find<Country>()!.MasterSource.Cast<Country>();
        private Country? _country;
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Country? country = _regions.FirstOrDefault(s => s.Equals(value))?.Country;
            _country = _countries.FirstOrDefault(s => s.Equals(country));
            return _country?.CountryName;
        }
    }

    public class CityConverter : IValueConverter
    {
        protected IEnumerable<City> _city = DatabaseManager.Find<City>()!.MasterSource.Cast<City>();
        
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _city.FirstOrDefault(s => s.Equals(value))?.CityName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("City converter");
        }
    }

    public class GetPersonFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            Person? person = sky.Person;
            
            if (sky.Horoscope != null) sky = sky.Horoscope;
            if (person == null || string.IsNullOrEmpty(person.ToString().Trim())) return "Today Sky";

            switch (sky.SkyType)
            {
                case SkyType.Sky:
                    return $"Sky of {person?.ToString()}";
                case SkyType.SunReturn:
                    return $"Sun Return of {person?.ToString()}";
                case SkyType.MoonReturn:
                    return $"Moon Return of {person?.ToString()}";
                case SkyType.Horoscope:
                    return $"Horoscope of {person?.ToString()}";
            }
            return $"Name: {person?.ToString()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetDateFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            return $"Date: {sky.LocalDateTime.ToString("dd/MM/yyyy")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetPlaceFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            return $"{sky.City.CityName}, {sky.City.Region.Country}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetPlaceCordFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            return $"Long: {Math.Round(sky.City.Longitude,2)}, Lat: {Math.Round(sky.City.Latitude, 2)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetTimeFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            Person? p = sky.Person;

            if (p != null && p.UnknownTime) 
                return "Unknown Time";

            int totalHours = (int)sky.LocalTime.TotalHours;
            int minutes = sky.LocalTime.Minutes;
            return $"Time: {totalHours:D2}:{minutes:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetUTFromSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            Person? p = sky.Person;

            if (p != null && p.UnknownTime)
                return "Unknown Time";

            int totalHours = (int)sky.UT.TotalHours;
            int minutes = sky.UT.Minutes;
            return $"UT: {Math.Abs(totalHours):D2}:{Math.Abs(minutes):D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetSideralSky : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SkyEvent sky = (SkyEvent)value;
            if (sky.Horoscope != null) sky = sky.Horoscope;
            Person? p = sky.Person;

            if (p != null && p.UnknownTime)
                return "Unknown Time";
            TimeSpan timeSpan = TimeSpan.FromHours(sky.SideralTime);

            int totalHours = (int)timeSpan.TotalHours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            return $"ST: {totalHours:D2}:{minutes:D2}:{seconds:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
