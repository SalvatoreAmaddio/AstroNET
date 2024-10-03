using AstroNET.model;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace AstroNET.controller
{
    public class SavedChartControllerList : AbstractFormListController<SavedCharts>
    {
        public override AbstractClause InstantiateSearchQry() =>
        new SavedCharts().Select().All().From();

        public SavedChartControllerList() 
        { 
            AllowNewRecord = false;
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<SavedCharts>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(SavedCharts model)
        {
            throw new NotImplementedException();
        }
    }
}
