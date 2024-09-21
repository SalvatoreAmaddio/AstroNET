using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class AspectListController : AbstractFormListController<Aspect>
    {
        public AspectListController()
        {
            AfterUpdate += OnAfterUpdate;
            AllowNewRecord = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new Aspect().Select().All().From().Where().Like("LOWER(AspectName)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<Aspect>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Aspect model)
        {
            new AspectWindow(model).ShowDialog();
        }
    }
}