using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    public interface IAstrologyAttribute
    {
        public Int64 ID();
    }

    [Table(nameof(Element))]
    public class Element : AbstractModel<Element>, IAstrologyAttribute
    {
        private Int64 _elementId;
        private string _elementName = string.Empty;

        [PK]
        public Int64 ElementId { get => _elementId; set => UpdateProperty(ref value, ref _elementId); }

        [Field]
        public string ElementName { get => _elementName; set => UpdateProperty(ref value, ref _elementName); }

        public Element()
        {

        }

        public Element(Int64 id)
        {
            _elementId = id;
        }

        public Element(DbDataReader reader)
        {
            _elementId = reader.GetInt64(0);
            _elementName = reader.GetString(1);
        }
        public Int64 ID() => _elementId;

        public override string ToString() => ElementName;
    }
}