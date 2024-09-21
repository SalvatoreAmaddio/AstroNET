using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.model;

namespace WpfApp1.controller
{
    public abstract class AbstractLibraryControllerList<M> : AbstractFormListController<M> where M : AbstractLibrary<M>, new()
    {

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

    }

    public class LibraryControllerList : AbstractLibraryControllerList<LibraryAspect>
    {
        public override AbstractClause InstantiateSearchQry()
        {
            return new LibraryAspect().Select().All().From().Where().EqualsTo("TransitId","@transitId");
        }

        public async override Task<IEnumerable<LibraryAspect>> SearchRecordAsync()
        {
            SearchQry.AddParameter("transitId", 1);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(LibraryAspect model)
        {
            throw new NotImplementedException();
        }
    }
}