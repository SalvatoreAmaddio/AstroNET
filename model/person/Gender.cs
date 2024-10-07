using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    public interface IGender
    {
        Int64 ID();
    }

    [Table(nameof(Gender))]
    public class Gender : AbstractModel<Gender>, IGender
    {
        private Int64 _genderId;
        private string _genderName = string.Empty;

        [PK]
        public Int64 GenderId { get => _genderId; set => UpdateProperty(ref value, ref _genderId); }

        [Field]
        public string GenderName { get => _genderName; set => UpdateProperty(ref value, ref _genderName); }

        public Gender()
        {

        }

        public Gender(Int64 id)
        {
            _genderId = id;
        }

        public Gender(DbDataReader reader)
        {
            _genderId = reader.GetInt64(0);
            _genderName = reader.GetString(1);
        }

        public override string ToString() => GenderName;

        public long ID() => _genderId;
    }
}