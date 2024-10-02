using System.Windows;
using System.Windows.Controls;

namespace AstroNET.View
{
    public class ZodiacTableTemplate : Control
    {
        static ZodiacTableTemplate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZodiacTableTemplate), new FrameworkPropertyMetadata(typeof(ZodiacTableTemplate)));
        }
    }
}
