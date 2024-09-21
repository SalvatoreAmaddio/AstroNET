using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class HouseListController : AbstractPointListController<House>
    {
        protected override void Open(House model)
        {
            new HouseWindow(model).ShowDialog();
        }
    }
}
