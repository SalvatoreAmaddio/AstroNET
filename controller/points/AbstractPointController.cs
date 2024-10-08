using FrontEnd.Controller;
using FrontEnd.Model;
using AstroNETLibrary.Points;

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