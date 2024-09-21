using FrontEnd.Controller;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public class AspectController : AbstractFormController<Aspect>
    {
        public AspectController() 
        { 
            AllowNewRecord = false;
        }
    }
}