using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.View
{
    public class AspectsGridTemplate : Control
    {
        static AspectsGridTemplate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AspectsGridTemplate), new FrameworkPropertyMetadata(typeof(AspectsGridTemplate)));
        }
    }
}
