using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public class SignListController : AbstractFormListController<Sign>
    {
        public override AbstractClause InstantiateSearchQry()
        {
            return new Sign().Select().All().From().Where().Like("LOWER(SignName)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<Sign>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Sign model)
        { }
    }
}
