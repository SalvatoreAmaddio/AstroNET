using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using AstroNET.model;

namespace AstroNET.controller
{
    public class StarTransitOrbitListController : AbstractFormListController<StarTransitOrbit>
    {
        public Aspect? Aspect { get; set; }
        public StarTransitOrbitListController() 
        {
            WindowLoaded += OnWindowLoaded;
            AllowNewRecord = false;
        }

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            var results = await SearchRecordAsync();
            RecordSource.ReplaceRecords(results);
            CurrentRecord = RecordSource.FirstOrDefault();
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new StarTransitOrbit().Select().From().Where().EqualsTo("AspectId","@aspectId");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<StarTransitOrbit>> SearchRecordAsync()
        {
            SearchQry.AddParameter("aspectId", Aspect?.AspectId);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(StarTransitOrbit model)
        {
        }
    }
}