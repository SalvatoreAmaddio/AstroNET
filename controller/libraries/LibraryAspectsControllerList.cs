using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;
using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public abstract class AbstractLibraryControllerList<M> : AbstractFormListController<M> where M : AbstractLibrary<M>, new()
    {
        protected TransitType TransitType { get; set; } = null!;
        public SourceOption StarOptions { get; private set; }
        public AbstractLibraryControllerList() 
        {
            StarOptions = new SourceOption(new RecordSource<Star>(DatabaseManager.Find<Star>()!), "PointName", OrderBy.ASC, "PointId");
            AfterUpdate += OnAfterUpdate;
            WindowLoaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            await OnSearchPropertyRequeryAsync(this);
        }

        public AbstractLibraryControllerList(TransitType transitType) : this()
        {
            TransitType = transitType;
        }

        public async override Task<IEnumerable<M>> SearchRecordAsync()
        {
            SearchQry.AddParameter("transitId", TransitType.TransitTypeId);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
                await OnSearchPropertyRequeryAsync(sender);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new M().
                Select().All()
                .From()
                .Where()
                    .EqualsTo("TransitTypeId", "@transitId");
        }
    }

    public class LibraryAspectsControllerList : AbstractLibraryControllerList<LibraryAspect>
    {
        public SourceOption StarOptions2 { get; private set; }
        public SourceOption EnergyOptions { get; private set; }

        public LibraryAspectsControllerList(TransitType transitType) : base(transitType)
        {
            StarOptions2 = new SourceOption(new RecordSource<Star>(DatabaseManager.Find<Star>()!), "PointName", OrderBy.ASC, "PointId");
            EnergyOptions = new SourceOption(new RecordSource<Energy>(DatabaseManager.Find<Energy>()!), "EnergyName");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            StarOptions.Conditions<WhereClause>(SearchQry,"PointId");
            StarOptions2.Conditions<WhereClause>(SearchQry,"StarID");
            EnergyOptions.Conditions<WhereClause>(SearchQry, "EnergyId");
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        protected override void Open(LibraryAspect model)
        {
            new LibraryAspectWindow(model).ShowDialog();
        }
    }
}