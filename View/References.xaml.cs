using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;

namespace AstroNET.View
{
    public partial class References : Window
    {
        public References() => InitializeComponent();

        private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string link = @"https://www.amazon.co.uk/stores/Ciro-Discepolo/author/B003DC8JOQ?ref=sr_ntt_srch_lnk_1&qid=1728315695&sr=1-1&isDramIntegrated=true&shoppingPortalEnabled=true";
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}