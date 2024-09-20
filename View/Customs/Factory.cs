using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.model;

namespace WpfApp1.View
{
    public class Factory
    {
        public static PencilCase PencilCase = new();
        public static Image MakePlanetImage(string uri) 
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(uri)),
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
        }

        public static StackPanel CreateStackPanel(string toolTipText = "") => new() { Height = 40, Width = 48, HorizontalAlignment = HorizontalAlignment.Center, ToolTip = toolTipText };
        public static void PlaceStackPanel(Canvas canvas, StackPanel stack, Point endPoint)
        {
            canvas.Children.Add(stack);
            Canvas.SetLeft(stack, endPoint.X - 48 / 2);
            Canvas.SetTop(stack, endPoint.Y - 40 / 2);
        }
        public static void PlaceHouse(Canvas canvas, House house, Point endPoint)
        {
            StackPanel stack = Factory.CreateStackPanel();
            stack.Children.Add(Factory.CreateHouseLabel(house.PointName));
            stack.Children.Add(Factory.CreateDegreeLabel(house.Position.DegreeAndMinutes));
            stack.ToolTip = $"{house} in {house.RadixSign} at {house.Position.DegreeAndMinutes}";
            Factory.PlaceStackPanel(canvas, stack, endPoint);
        }

        private static Label InitLabel(string text) => new()
        {
            Content = text,
            BorderBrush = Brushes.Black,
            BorderThickness = new(0),
            Margin = new(),
            Padding = new(),
            Width = 48,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Height = 20,
        };
        public static Label CreateDegreeLabel(string text)
        {
            Label label = InitLabel(text.Trim());
            label.VerticalContentAlignment = VerticalAlignment.Top;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            //label.FontWeight = FontWeights.Bold;
            return label;
        }

        public static Label CreateHouseLabel(string text)
        {
            Label label = InitLabel(text);
            label.VerticalContentAlignment = VerticalAlignment.Bottom;
            //if (isAngle) label.FontWeight = FontWeights.Bold;
            return label;
        }
        public static bool IsMultipleOfFive(double number) => number % 5 == 0;

        public static Line MakeOverlayLine(Point startPoint, Point endPoint, string toolTipText)
        {
            Line line = new Line()
            {
                Stroke = Brushes.Transparent,
                StrokeThickness = 5,
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y
            };

            ToolTip toolTip = new()
            {
                Content = toolTipText
            };
            ToolTipService.SetToolTip(line, toolTip);
            return line;
        }

        public static void PlaceImage(string imgPath, Point endPoint, double outerRadius, DrawingContext drawingContext)
        {
            ImageSource imageSource = new BitmapImage(new Uri(imgPath));
            double imageWidth = outerRadius / 8;
            double imageHeight = outerRadius / 8;

            Rect imageRect = new(
                endPoint.X - imageWidth / 2,
                endPoint.Y - imageHeight / 2,
                imageWidth,
                imageHeight);

            drawingContext.DrawImage(imageSource, imageRect);
        }
    }
}
