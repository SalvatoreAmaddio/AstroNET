using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class StarListController : AbstractPointListController<Star>
    {

        protected override void Open(Star model)
        {
            new StarWindow(model).ShowDialog();
        }
    }
}
