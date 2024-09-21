using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using WpfApp1.model;
using Backend.ExtensionMethods;

namespace WpfApp1.controller
{
    public class CityListController : AbstractFormListController<City>
    {
        public CityListController() 
        {
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new City().Select().All().From().Where().Like("LOWER(CityName)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<City>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(City model)
        {
        }
    }
}
