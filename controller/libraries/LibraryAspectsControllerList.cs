using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;
using System.Windows;
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

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e) =>
        await OnSearchPropertyRequeryAsync(this);

        public AbstractLibraryControllerList(TransitType transitType) : this() =>
        TransitType = transitType;

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

        public abstract void SetTitle();
    }

    public class LibraryAspectsControllerList : AbstractLibraryControllerList<LibraryAspects>
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

        protected override void Open(LibraryAspects model)
        {
            if (model.IsNewRecord()) 
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryAspectWindow(model).ShowDialog();
        }

        public override void SetTitle()
        {
            long transitID = TransitType.TransitTypeId;

            switch (transitID)
            {
                case 1:
                    ((Window?)UI).Title = "Radix Aspects";
                    break;
                case 2:
                    ((Window?)UI).Title = "Transits Aspects";
                    break;
                case 3:
                    ((Window?)UI).Title = "Sinastry Aspects";
                    break;
            }
        }
    }

    public class LibraryHousesControllerList : AbstractLibraryControllerList<LibraryHouses> 
    {
        public SourceOption HouseOptions { get; private set; }
        public LibraryHousesControllerList(TransitType transitType) : base(transitType)
        {
            HouseOptions = new SourceOption(new RecordSource<House>(DatabaseManager.Find<House>()!), "PointName", OrderBy.ASC, "PointId");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            StarOptions.Conditions<WhereClause>(SearchQry, "PointId");
            HouseOptions.Conditions<WhereClause>(SearchQry, "HouseId");
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        protected override void Open(LibraryHouses model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryHouseWindow(model).ShowDialog();
        }

        public override void SetTitle()
        {
            long transitID = TransitType.TransitTypeId;

            switch (transitID)
            {
                case 1:
                    ((Window?)UI).Title = "Radix Houses";
                    break;
                case 2:
                    ((Window?)UI).Title = "Transits in Houses";
                    break;
                case 3:
                    ((Window?)UI).Title = "Sinastry Houses";
                    break;
                case 4:
                    ((Window?)UI).Title = "Return Houses";
                    break;
            }
        }
    }

    public class LibrarySignsControllerList : AbstractLibraryControllerList<LibrarySigns>
    {
        public SourceOption SignOptions { get; private set; }
        public LibrarySignsControllerList(TransitType transitType) : base(transitType)
        {
            SignOptions = new SourceOption(new RecordSource<Sign>(DatabaseManager.Find<Sign>()!), "SignName", OrderBy.ASC, "SignId");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            StarOptions.Conditions<WhereClause>(SearchQry, "PointId");
            SignOptions.Conditions<WhereClause>(SearchQry, "SignId");
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        protected override void Open(LibrarySigns model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibrarySignWindow(model).ShowDialog();
        }

        public override void SetTitle()
        {
            long transitID = TransitType.TransitTypeId;

            switch (transitID)
            {
                case 1:
                    ((Window?)UI).Title = "Star in Sign";
                    break;
                case 2:
                    ((Window?)UI).Title = "Transits in Houses";
                    break;
                case 3:
                    ((Window?)UI).Title = "Sinastry Houses";
                    break;
                case 4:
                    ((Window?)UI).Title = "Return Houses";
                    break;
            }
        }
    }

}