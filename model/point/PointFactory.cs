namespace AstroNET.model.point
{
    public class PointFactory
    {
        public static Func<int, double, IHouse>? CreateHouse;

        public static Func<int, double[], IStar>? CreateStar;

        public static Func<int, double[], double[], IStar>? CreateStar2;
    }
}
