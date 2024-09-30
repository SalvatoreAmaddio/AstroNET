using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.View
{
    public class ZodiacTableTemplate : Control
    {
        static ZodiacTableTemplate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZodiacTableTemplate), new FrameworkPropertyMetadata(typeof(ZodiacTableTemplate)));
        }
    }
}
