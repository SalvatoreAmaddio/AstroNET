using System.Windows.Media;

namespace WpfApp1.View
{
    public class PencilCase
    {
        public Pen RedPen = new(Brushes.Red, .2);
        public Pen BluePen1 = new(Brushes.Blue, 1);
        public Pen BlackPen2 = new(Brushes.Black, 2);
        public Pen BlackPen1 = new(Brushes.Black, 1);
        public Pen BlackPen05 = new(Brushes.Black, .5);
        public Pen BlackPenForHouse = new(Brushes.Black, .4);
        public static Pen MakePen(Brush brush, double thickness = 1, bool isDashed = false) 
        {
            return (isDashed) ? new(brush, thickness) { DashStyle = new DashStyle(new double[] { 4, 4 }, 0) } : new(brush, thickness);
        }
    }
}