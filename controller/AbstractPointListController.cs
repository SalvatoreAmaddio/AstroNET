using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public abstract class AbstractPointListController<M> : AbstractFormListController<M> where M : IAbstractModel, IPoint, new()
    {
        public override AbstractClause InstantiateSearchQry()
        {
            return new M().Select().All().From().Where().Like("LOWER(PointName)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<M>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(M model)
        {
        }
    }
}
