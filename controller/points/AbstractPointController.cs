using FrontEnd.Controller;
using FrontEnd.Model;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public abstract class AbstractPointController<M> : AbstractFormController<M> where M : IAbstractModel, IPoint, new()
    {
        public AbstractPointController() 
        {
            AllowNewRecord = false;
        }
    }
}