﻿using Backend.Database;
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
            bool isRetrograde;
            if (value is House) return string.Empty;
            if (value is Star star)
            {
                isRetrograde = star.IsRetrograde;
            }
            else
            {
                isRetrograde = (bool)value;
            }
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
            return genders.FirstOrDefault(s => s.Equals(value))?.ToString();
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
            return _regions.FirstOrDefault(s => s.Equals(value))?.RegionName;
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

    public abstract class GetFromSky : IValueConverter
    {
        public abstract object? Convert(object value, Type targetType, object parameter, CultureInfo culture);

        protected static void ReplaceSky(ref AbstractSkyEvent sky)
        {
            if (sky is SkyEvent s && s.Horoscope != null) sky = s.Horoscope;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetPersonFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;
            Person? person = sky.Person;

            ReplaceSky(ref sky);

            if (person == null || string.IsNullOrEmpty(person.ToString().Trim())) return "Today Sky";

            switch (sky.SkyInfo.SkyType)
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

    }

    public class GetDateFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;
            ReplaceSky(ref sky);
            return $"Date: {sky.SkyInfo.LocalDateTime.ToString("dd/MM/yyyy")}";
        }
    }

    public class GetPlaceFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;
            ReplaceSky(ref sky);
            return $"{sky.SkyInfo.City.CityName}, {sky.SkyInfo.City.Region.Country}";
        }
    }

    public class GetPlaceCordFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;
            ReplaceSky(ref sky);
            return $"Long: {Math.Round(sky.SkyInfo.City.Longitude, 2)}, Lat: {Math.Round(sky.SkyInfo.City.Latitude, 2)}";
        }
    }

    public class GetTimeFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;

            ReplaceSky(ref sky);

            Person? p = sky.Person;

            if (p != null && p.UnknownTime)
                return "Unknown Time";

            int totalHours = (int)sky.SkyInfo.LocalTime.TotalHours;
            int minutes = sky.SkyInfo.LocalTime.Minutes;
            return $"Time: {totalHours:D2}:{minutes:D2}";
        }
    }

    public class GetUTFromSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;

            ReplaceSky(ref sky);

            Person? p = sky.Person;

            if (p != null && p.UnknownTime)
                return "Unknown Time";

            int totalHours = (int)sky.SkyInfo.UT.TotalHours;
            int minutes = sky.SkyInfo.UT.Minutes;
            return $"UT: {Math.Abs(totalHours):D2}:{Math.Abs(minutes):D2}";
        }
    }

    public class GetSideralSky : GetFromSky
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AbstractSkyEvent sky = (AbstractSkyEvent)value;
            ReplaceSky(ref sky);

            Person? p = sky.Person;

            if (p != null && p.UnknownTime)
                return "Unknown Time";
            TimeSpan timeSpan = TimeSpan.FromHours(sky.SkyInfo.SideralTime);

            int totalHours = (int)timeSpan.TotalHours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            return $"ST: {totalHours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}
