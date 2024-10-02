namespace AstroNET.model
{
    public interface IPoint
    {
        Int64 PointId { get; }
        string PointName { get; set; }
        string Description { get; set; }
        double EclipticLongitude { get; }
        Position Position { get; }
        Sign RadixSign { get; }
        void Build();
        House PlaceInHouse(AbstractSkyEvent sky);
    }

    public interface IHouse : IPoint
    {
        Sign RullerSign { get; }
        bool IsAngular { get; }
    }

    public interface IStar : IPoint
    {
        IHouse House { get; }
        Element Element { get; }
        bool IsRetrograde { get; }
        public string URI { get; }
        double TransitOrbit { get; }
    }
}
