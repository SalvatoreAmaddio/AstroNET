using FrontEnd.Controller;
using FrontEnd.Model;
using AstroNET.model;

namespace AstroNET.controller
{
    public abstract class AbstractPointController<M> : AbstractFormController<M> where M : IAbstractModel, IPoint, new()
    {
        public AbstractPointController() 
        {
            AllowNewRecord = false;
        }
    }
}