using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;

namespace AstroNET.View
{
    public class Utils
    {
        public static InlineUIContainer LineSeparator() 
        {
            return new InlineUIContainer(
                new Border()
                { 
                    BorderBrush = System.Windows.Media.Brushes.Black, 
                    BorderThickness = new(1), 
                    Width = double.NaN,
                    Margin = new Thickness(0, 10, 0, 10)
                });
        }
        public static InlineUIContainer? Img(string? uri, string? toolTip = "")
        {
            if (string.IsNullOrEmpty(uri)) return null;
            Image image = new()
            {
                Source = new BitmapImage(new Uri(uri)),
                Width = 20, // Set image size
                Height = 20,
                VerticalAlignment = VerticalAlignment.Bottom,
                ToolTip = toolTip
            };
            return new InlineUIContainer(image);
        }
    }
}
