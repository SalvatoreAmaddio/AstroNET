using AstroNET.model;
using AstroNET.View;

namespace AstroNET.controller
{
    public class StarListController : AbstractPointListController<Star>
    {

        protected override void Open(Star model)
        {
            new StarWindow(model).ShowDialog();
        }
    }
}
