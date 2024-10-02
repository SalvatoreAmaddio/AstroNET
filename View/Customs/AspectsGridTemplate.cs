using System.Windows;
using System.Windows.Controls;

namespace AstroNET.View
{
    public class AspectsGridTemplate : Control
    {
        static AspectsGridTemplate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AspectsGridTemplate), new FrameworkPropertyMetadata(typeof(AspectsGridTemplate)));
        }
    }
}
