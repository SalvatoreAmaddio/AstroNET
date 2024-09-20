using System.Data.Common;
using Backend.Database;
using Backend.Model;
using FrontEnd.Model;

namespace WpfApp1.model
{
    [Table(nameof(TimeZone))]
    public class TimeZone : AbstractModel<TimeZone>, ILocation
    {
        private Int64 _timeZoneId;
        private string _timeZoneName = string.Empty;

        [PK]
        public Int64 TimeZoneId { get => _timeZoneId; set => UpdateProperty(ref value, ref _timeZoneId); }

        [Field]
        public string TimeZoneName { get => _timeZoneName; set => UpdateProperty(ref value, ref _timeZoneName); }

        public TimeZoneInfo Info { get; private set; } = TimeZoneInfo.Local;

        public TimeZone() { }
        public TimeZone(Int64 id) => _timeZoneId = id;
        public TimeZone(DbDataReader reader)
        {
            _timeZoneId = reader.GetInt64(0);
            _timeZoneName = reader.GetString(1);
        }

        public void Build()
        {
            TimeZone tempTimeZone = DatabaseManager.Find<TimeZone>()!.MasterSource.Cast<TimeZone>().First(s => s.Equals(this));
            _timeZoneName = tempTimeZone.TimeZoneName;

            try
            {
                Info = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ReadLocation(Location location)
        {
            TimeZoneName = location.TimeZoneId;
        }
        public override string? ToString() => $"{TimeZoneName}";
    }
}