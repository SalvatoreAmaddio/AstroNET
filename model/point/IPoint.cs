namespace AstroNET.model
{
    public interface IPoint
    {
        Int64 PointId { get; }
        string PointName { get; set; }
        string Description { get; set; }
        double EclipticLongitude { get; }
        Position Position { get; }
        ISign GetRadixSign();
        void Build();
        IHouse PlaceInHouse(AbstractSkyEvent sky);
    }

    public interface IHouse : IPoint
    {
        ISign GetRullerSig();
        bool IsAngular { get; }
        public static long SlidHouse(double orbDiff, IHouse house)
        {
            if (orbDiff < 0)
            {
                return (house.PointId == 1) ? 12 : house.PointId - 1;
            }

            return (house.PointId == 12) ? 1 : house.PointId + 1;
        }
    }

    public interface IStar : IPoint
    {
        IHouse House { get; }
        IAstrologyAttribute GetElement();
        bool IsRetrograde { get; }
        public string URI { get; }
        double TransitOrbit { get; }
    }
}
