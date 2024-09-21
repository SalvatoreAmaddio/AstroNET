using FrontEnd.Controller;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public class SignController : AbstractFormController<Sign>
    {
        public SignController() 
        {
            AllowNewRecord = false;
        }
    }
}
