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
        public AbstractPointListController() 
        {
            AfterUpdate += OnAfterUpdate;
            AllowNewRecord = false;
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new M().Select().All().From().Where().Like("LOWER(PointName)", "@name");
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<M>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }


    }
}
