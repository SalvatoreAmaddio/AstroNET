using SwissEphNet;

namespace WpfApp1.model
{
    public class DateTimeAdjuster
    {
        private const long one_second = 10000000;
        private const long one_hour = 36000000000;
        private const long one_day = 864000000000;
        private const long one_minute = 600000000;
        private long _ticks;
        private DateTime _inputDate;

        public DateTime InputDate
        {
            get => _inputDate;
            private set
            {
                _inputDate = value;
                Ticks = _inputDate.Ticks;
            }
        }

        public TimeSpan OutputTime => new(OutputHour, OutputMinute, OutputSecond);
        public DateTime OutputDate => new(Ticks);
        public int OutputYear => OutputDate.Year;
        public int OutputMonth => OutputDate.Month;
        public int OutputDay => OutputDate.Day;
        public int OutputHour => OutputDate.Hour;
        public int OutputMinute => OutputDate.Minute;
        public int OutputSecond => OutputDate.Second;
        public long Ticks
        {
            get => _ticks;
            private set => _ticks = value;
        }

        public DateTimeAdjuster(DateTime date) => InputDate = date;

        public void AddHours(int hours = 1) => Ticks += hours * one_hour;
        public void RemoveHours(int hours = 1) => Ticks -= hours * one_hour;
        public void AddMinutes(int minutes = 1) => Ticks += minutes * one_minute;
        public void RemoveMinutes(int minutes = 1) => Ticks -= minutes * one_minute;
        public void AddDays(int days = 1) => Ticks += days * one_day;
        public void RemoveDays(int days = 1) => Ticks -= days * one_day;
        public void AddSeconds(int seconds = 1) => Ticks += seconds * one_second;
        public void RemoveSeconds(int seconds = 1) => Ticks -= seconds * one_second;
        public override string ToString() => OutputDate.ToString("yyyy-MM-dd HH:mm:ss");

    }
    public class PositionCalculator : SwissEph
    {
        private double[] cusps = new double[13];
        private double[] ascmc = new double[10];
        private double[] xx = new double[6];
        private string serr = string.Empty;
        private int iflag = SwissEph.SEFLG_SWIEPH | SwissEph.SEFLG_SPEED;
        private SkyEvent _skyEvent;
        public PositionCalculator(SkyEvent skyEvent) =>
        _skyEvent = skyEvent;

        public IEnumerable<Aspect> TransitsCalculator(DateTime startDate, City city, int starId, int steps)
        {
            DateTime endDate = startDate.AddMonths(steps);
            DateTimeAdjuster startDateAdj = new(startDate);
            SkyEvent currentSky;
            Star star;
            List<Aspect> aspects = [];

            while (true) 
            {
                currentSky = new(startDateAdj.OutputDate, startDateAdj.OutputTime, city);
                star = GetStarPosition(ref currentSky, starId);                
                aspects.AddRange(_skyEvent.CalculateStarAspects(star, startDateAdj.OutputDate));
                startDateAdj.AddDays(1);
                if (startDateAdj.OutputDate > endDate) return aspects;
            }
        }

        public async Task<List<Aspect>> TransitsCalculatorAsync(DateTime startDate, City city, int starId, int steps)
        {
            List<Task<List<Aspect>>> tasks = [];
            List<Aspect> aspects = [];

            foreach (DateTime date in DateList(startDate, steps))
            {
                Task<List<Aspect>> task = Task.Run(() => CalculateMonthlyTransitAsync(date, starId, city));
                tasks.Add(task);
            }

            foreach (List<Aspect>? monthlyAspect in await Task.WhenAll(tasks))
            {
                aspects.AddRange(monthlyAspect);
            }

            return aspects;
        }

        private static IEnumerable<DateTime> DateList(DateTime startDate, int steps) 
        {
            int months = 0;
            do
            {
                yield return startDate;
                months++;
                startDate = startDate.AddMonths(1);
            } while (months < steps);
        }

        private Task<List<Aspect>> CalculateMonthlyTransitAsync(DateTime startDate, int starId, City city) 
        {
            DateTimeAdjuster startDateAdj = new(startDate);
            DateTime endDate = startDate.AddMonths(1);
            SkyEvent currentSky;
            Star star;
            List<Aspect> aspects = [];

            while (true)
            {
                currentSky = new(startDateAdj.OutputDate, startDateAdj.OutputTime, city);
                star = GetStarPosition(ref currentSky, starId);
                aspects.AddRange(_skyEvent.CalculateStarAspects(star, startDateAdj.OutputDate));
                startDateAdj.AddDays(1);
                if (startDateAdj.OutputDate > endDate) return Task.FromResult(aspects);
            }
        }

        private static int CalculateMoonDegrees(double minutes)
        {
            double target = 60;
            int min = 0;

            while (minutes < target)
            {
                minutes++;
                min+=3;
            }

            return min;
        }

        public (DateTime date, TimeSpan time) MoonReturn(DateTime returnDate, City city)
        {
            SkyEvent returnSky;
            Star moonReturn;
            Star moonRadix = _skyEvent.Stars[1];

            DateTimeAdjuster adjuster = new(returnDate);

            while (true)
            {

                returnSky = new(adjuster.OutputDate, adjuster.OutputTime, city);
                moonReturn = GetStarPosition(ref returnSky, 1);

                if (moonReturn.RadixSign.SignId != moonRadix.RadixSign.SignId)
                {
                    if (Math.Abs(moonReturn.RadixSign.SignId - moonRadix.RadixSign.SignId) > 1) 
                    {
                        adjuster.AddDays(2);
                    }
                    else 
                    {
                        adjuster.AddDays(1);
                    }
                    continue;
                }

                var x = moonReturn.Position.Minutes;
                
                if (moonReturn.Position.Degrees < moonRadix.Position.Degrees) 
                {
                    adjuster.AddMinutes(CalculateMoonDegrees(x));
                    continue;
                }

                if (moonReturn.Position.Degrees > moonRadix.Position.Degrees)
                {
                    adjuster.RemoveMinutes(CalculateMoonDegrees(x));
                    continue;
                }

                if (moonReturn.Position.Minutes > moonRadix.Position.Minutes)
                {
                    adjuster.RemoveMinutes(1);
                    continue;
                }

                if (moonReturn.Position.Minutes < moonRadix.Position.Minutes)
                {
                    adjuster.AddMinutes(1);
                    continue;
                }

                return (adjuster.OutputDate, adjuster.OutputTime);

            }
        }
       
        public (DateTime date, TimeSpan time) CalculateSunReturn(int year, City city)
        {
            SkyEvent returnSky;
            Star sunReturn;
            
            DateTimeAdjuster adjuster = new(new(year, _skyEvent.Month, _skyEvent.Day));

            Star sunRadix = _skyEvent.Stars[0];

            while (true)
            {
                returnSky = new(adjuster.OutputDate, adjuster.OutputTime, city);
                sunReturn = GetStarPosition(ref returnSky);

                if (sunReturn.Position.Degrees > sunRadix.Position.Degrees)
                {
                    adjuster.RemoveMinutes(60);
                    continue;
                }

                if (sunReturn.Position.Degrees < sunRadix.Position.Degrees)
                {
                    adjuster.AddMinutes(60);
                    continue;
                }

                if (sunReturn.Position.Minutes < sunRadix.Position.Minutes && sunReturn.Position.Degrees == sunRadix.Position.Degrees)
                {
                    adjuster.AddMinutes(CalculateSunMinutes(sunReturn.Position.Minutes));
                    continue;
                }

                if (sunReturn.Position.Minutes > sunRadix.Position.Minutes && sunReturn.Position.Degrees == sunRadix.Position.Degrees)
                {
                    adjuster.RemoveMinutes(CalculateSunMinutes(sunReturn.Position.Minutes));
                    continue;
                }
                
                if (sunReturn.Position.Seconds < sunRadix.Position.Seconds && sunReturn.Position.Minutes == sunRadix.Position.Minutes && sunReturn.Position.Degrees == sunRadix.Position.Degrees)
                {
                    adjuster.AddSeconds(CalculateSecondsMatch(sunReturn.Position.Seconds, sunRadix.Position.Seconds));
                    continue;
                }

                double diff = sunRadix.Position.Seconds - sunReturn.Position.Seconds;
                if (diff >= -2 && diff <= 4) return (adjuster.OutputDate, adjuster.OutputTime);

                if (sunReturn.Position.Seconds > sunRadix.Position.Seconds && sunReturn.Position.Minutes == sunRadix.Position.Minutes && sunReturn.Position.Degrees == sunRadix.Position.Degrees)
                {
                    adjuster.RemoveSeconds(CalculateSeconds(sunReturn.Position.Seconds));
                    continue;
                }

                return (adjuster.OutputDate, adjuster.OutputTime);                
            }
        }

        private static int CalculateSeconds(double seconds)
        {
            double target = 60;
            int sec = 0;

            while (seconds < target)
            {
                seconds++;
                sec+=10;
            }

            return sec;
        }

        private static int CalculateSecondsMatch(double seconds, double target)
        {
            int sec = 0;

            while (seconds < target)
            {
                seconds++;
                sec+=10;
            }

            return sec;
        }

        private static int CalculateSunMinutes(double seconds) 
        {
            double target = 60;
            int min = 0;

            while (seconds < target) 
            {
                seconds += 3;
                min++;
            }

            return min;
        }

        private static Star GetStarPosition(ref SkyEvent returnSky, int star = 0) 
        {
            PositionCalculator returnCalculator = new(returnSky);
            return returnCalculator.CalculatePlanet(star);
        }

        public IEnumerable<(DateTime date, TimeSpan time)> CalculateMoonReturns(int year)
        {
            List<(DateTime date, TimeSpan time)> returns = [];
            (DateTime returnDate, TimeSpan returnTime) = CalculateSunReturn(year, _skyEvent.City);
            (DateTime nextReturnDate, TimeSpan nextReturnTime) = CalculateSunReturn(year + 1, _skyEvent.City);
            nextReturnDate = nextReturnDate.AddHours(nextReturnTime.Hours);
            nextReturnDate = nextReturnDate.AddMinutes(nextReturnTime.Minutes);
            Star moonReturn;
            Star moonRadix = CalculatePlanet(1);
            SkyEvent returnSky = new();


            while (true)
            {
                var temp = new DateTime(returnDate.Year, returnDate.Month, returnDate.Day, returnTime.Hours, returnTime.Minutes, returnTime.Seconds);
                if (temp >= nextReturnDate)
                {
                    return returns;
                }

                returnSky.Calculate(returnDate, returnTime, _skyEvent.City);

                PositionCalculator returnCalculator = new(returnSky);
                moonReturn = returnCalculator.CalculatePlanet(1);

                if (moonRadix.Position.Equals(moonReturn.Position) && moonRadix.RadixSign.SignId == moonReturn.RadixSign.SignId)
                {
                    returns.Add((returnDate, returnTime));
                    returnDate = returnDate.AddDays(26);
                    returnTime = TimeSpan.Zero;
                }

                returnTime = returnTime.Add(TimeSpan.FromMinutes(1));

                // Check if we overflowed into the next day
                if (returnTime.TotalHours >= 24)
                {
                    // If returnTime exceeds 24 hours, reset the time and increment the date
                    returnTime = TimeSpan.Zero;
                    returnDate = returnDate.AddDays(1);
                }
            }
        }

        public Star CalculatePlanet(int starID, bool ignoreHouse = false)
        {
            int result = swe_calc_ut(_skyEvent.JulianDay, starID, iflag, xx, ref serr);
            if (result < 0)
                throw new Exception("Error calculating the Sun's position: " + serr);
            Star star;

            if (ignoreHouse)
            {
                star = new(starID, ref xx);
            }
            else
            {
                swe_houses(_skyEvent.JulianDay, _skyEvent.City.Latitude, _skyEvent.City.Longitude, 'P', cusps, ascmc);
                star = new(starID, ref xx, ref cusps);
            }

            return star;
        }
        
        public void CalculateHouses()
        {
            swe_houses(_skyEvent.JulianDay, _skyEvent.City.Latitude, _skyEvent.City.Longitude, 'P', cusps, ascmc);
        }

        public House GetHouse(int houseId)
        {
            return new(houseId, cusps[houseId]);
        }

        public static Aspect? IsValidAspect(Aspect aspect, double distance, double tollerance = -1)
        {
            if (tollerance == -1)
                tollerance = aspect.NatalTollerance;

            if (IsWithinRange(Math.Abs(distance), aspect.Orbit, tollerance))
            {
                return aspect.Clone(distance);
            }

            return null;
        }

        private static bool IsWithinRange(double value, double target, double tolerance)
        {
            return value >= (target - tolerance) && value <= (target + tolerance);
        }

        //public static double CalculateDistance(double point1, double point2)
        //{
        //    double directDistance = Math.Abs(point1 - point2);

        //    // Calculate wrap-around distance
        //    double wrapAroundDistance = 360.0 - directDistance;

        //    return Math.Min(directDistance, wrapAroundDistance);
        //}

        public static double CalculateDistance(double point1, double point2)
        {
            point1 = point1 % 360;
            point2 = point2 % 360;

            // Calculate clockwise and counterclockwise distances
            double clockwiseDistance = (point2 - point1 + 360) % 360;
            double counterclockwiseDistance = (point1 - point2 + 360) % 360;

            // Determine the shortest distance (positive for clockwise, negative for counterclockwise)
            double shortestDistance = clockwiseDistance <= counterclockwiseDistance ? clockwiseDistance : -counterclockwiseDistance;

            return shortestDistance;
        }
    }

}
