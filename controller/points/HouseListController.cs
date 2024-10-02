using AstroNET.model;
using AstroNET.View;

namespace AstroNET.controller
{
    public class HouseListController : AbstractPointListController<House>
    {
        protected override void Open(House model)
        {
            new HouseWindow(model).ShowDialog();
        }
    }
}
