using FrontEnd.Controller;
using AstroNET.model;

namespace AstroNET.controller
{
    public class AspectController : AbstractFormController<Aspect>
    {
        public AspectController() 
        { 
            AllowNewRecord = false;
        }
    }
}