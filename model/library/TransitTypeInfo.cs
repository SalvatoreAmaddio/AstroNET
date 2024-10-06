namespace AstroNET.model
{
    public class TransitTypeRoot
    {
        public List<TransitTypeInfo> TransitTypeInfo { get; set; } = [];
    }

    public class TransitTypeInfo
    {
        public long TransitTypeId { get; set; }
        public int StarId { get; set; }
        public string StarName { get; set; } = string.Empty;
        public string TransitDescription { get; set; } = string.Empty;
        public TransitTypeInfo() { }

        public override string ToString() => $"{StarName}";

        public override bool Equals(object? obj) =>
        obj is TransitTypeInfo info &&
        TransitTypeId == info.TransitTypeId &&
        StarId == info.StarId;

        public override int GetHashCode() => HashCode.Combine(TransitTypeId, StarId);
    }

}
