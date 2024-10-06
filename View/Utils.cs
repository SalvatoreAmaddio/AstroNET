using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows;
using Image = System.Windows.Controls.Image;

namespace AstroNET.View
{
    public class Utils
    {

        public static Paragraph DoubleSpaceParagraph()
        {
            return DescriptionParagraph("\n\n");
        }

        public static Paragraph DescriptionParagraph(string? text, FontWeight? fontWeight = null)
        {
            if (fontWeight == null)
            {
                fontWeight = FontWeights.Normal;
            }

            Paragraph paragraph = new()
            {
                FontWeight = fontWeight.Value
            };

            paragraph.Inlines.Add(new Run(text));
            return paragraph;
        }

        public static Paragraph ImageParagraph(string? uri, string? toolTip = "")
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(InLineImage(uri, toolTip));
            return paragraph;
        }

        public static InlineUIContainer? InLineImage(string? uri, string? toolTip = "")
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
