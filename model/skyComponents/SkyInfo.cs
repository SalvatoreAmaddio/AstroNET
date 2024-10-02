using NodaTime;
using SwissEphNet;

namespace AstroNET.model
{
    public class SkyInfo
    {
        public SkyType SkyType { get; set; } = SkyType.Sky;
        public int SkyTypeId => SkyType switch
        {
            SkyType.Sky => 1,
            SkyType.Horoscope => 2,
            SkyType.Sinastry => 3,
            SkyType.SunReturn or SkyType.MoonReturn => 4,
            _ => -1,
        };

        public double JulianDay { get; private set; }
        public double HourUT { get; private set; }
        public TimeSpan UT
        {
            get
            {
                int hours = (int)Math.Floor(HourUT);
                int minutes = (int)((HourUT - hours) * 60);
                return new TimeSpan(hours, minutes, 0);
            }
        }
        public TimeSpan LocalTime { get; private set; }
        public DateTime LocalDateTime { get; set; }
        public double LocalHour { get; private set; }
        public City City { get; private set; } = null!;
        public int Year => LocalDateTime.Year;
        public int Month => LocalDateTime.Month;
        public int Day => LocalDateTime.Day;
        public double SideralTime { get; private set; }
        public string TimeString
        {
            get
            {
                int totalHours = (int)LocalTime.TotalHours;
                int minutes = LocalTime.Minutes;
                return $"Time: {totalHours:D2}:{minutes:D2}";
            }
        }

        public string UTString
        {
            get
            {
                int totalHours = (int)UT.TotalHours;
                int minutes = UT.Minutes;
                return $"UT: {Math.Abs(totalHours):D2}:{Math.Abs(minutes):D2}";
            }
        }

        public string SideralTimeString
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.FromHours(SideralTime);
                int totalHours = (int)timeSpan.TotalHours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;
                return $"ST: {totalHours:D2}:{minutes:D2}:{seconds:D2}";
            }
        }

        public SkyInfo() { }
        public SkyInfo(SkyType skyType) => SkyType = skyType;

        public void Calculate(int year, int month, int day, int hour, int minutes, City city)
        {
            City = city;
            City.Build();
            LocalHour = hour + minutes / 60.0;
            LocalTime = new(hour, minutes, 0);
            LocalDateTime = new DateTime(year, month, day, (int)LocalHour, (int)((LocalHour - (int)LocalHour) * 60), 0);

            bool isDaylight = CheckDST(LocalDateTime, City.TimeZone.TimeZoneName);

            TimeSpan timeOffset = City.TimeZone.Info.BaseUtcOffset;

            if (isDaylight)
                timeOffset = timeOffset.Add(new TimeSpan(1, 0, 0));

            HourUT = LocalHour - timeOffset.TotalHours;
            CalculateJulianDate();
        }
        public void Calculate(DateTime date, TimeSpan time, City city) =>
        Calculate(date.Year, date.Month, date.Day, time.Hours, time.Minutes, city);

        private static bool CheckDST(DateTime dateTime, string timeZoneId)
        {
            // Convert DateTime to NodaTime LocalDateTime
            LocalDateTime localDateTime = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);

            // Get the time zone from the IANA Time Zone Database
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

            // Convert LocalDateTime to ZonedDateTime
            ZonedDateTime zonedDateTime = timeZone.AtLeniently(localDateTime);

            // Check if the date is in DST
            return zonedDateTime.IsDaylightSavingTime();
        }

        private void CalculateJulianDate()
        {
            SwissEph sw = new();
            JulianDay = sw.swe_julday(Year, Month, Day, HourUT, SwissEph.SE_GREG_CAL);
            SideralTime = sw.swe_sidtime(JulianDay) + (City.Longitude / 15.0);
            if (SideralTime < 0) SideralTime += 24;
        }

        public override string ToString() =>
        $"{Day:00}/{Month:00}/{Year} at {LocalTime.ToString(@"hh\:mm")} (Location: {City.CityName}, {City.Region.Country} - Lat: {City.Latitude}°, Long: {City.Longitude}°)";

    }

}
