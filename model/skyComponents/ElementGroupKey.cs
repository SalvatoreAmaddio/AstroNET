namespace WpfApp1.model
{
    public class ElementGroupKey
    {
        public string Name { get; private set; } = string.Empty;
        public int Count { get; private set; }
        public IPoint? Point { get; private set; }
        public ElementGroupKey(IPoint point, int count)
        {
            Point = point;
            Name = Point.PointName;
            Count = count;
        }

        public ElementGroupKey(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override string? ToString() => $"{Name} - {Count}";
    }

}