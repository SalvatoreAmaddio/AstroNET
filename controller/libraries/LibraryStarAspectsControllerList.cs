using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;
using System.Windows;
using AstroNET.model;
using AstroNET.View;

namespace AstroNET.controller
{
    public abstract class AbstractPointLibraryControllerList<M> : AbstractFormListController<M> where M : AbstractPointLibrary<M>, IAbstractPointLibrary, new()
    {
        protected TransitType TransitType { get; set; } = null!;

        public AbstractPointLibraryControllerList()
        {
            AfterUpdate += OnAfterUpdate;
            WindowLoaded += OnWindowLoaded;
        }

        public AbstractPointLibraryControllerList(TransitType transitType) : this() =>
        TransitType = transitType;

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e) =>
        await OnSearchPropertyRequeryAsync(this);

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
        public async override Task<IEnumerable<M>> SearchRecordAsync()
        {
            SearchQry.AddParameter("transitId", TransitType.TransitTypeId);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public abstract void SetTitle();
    }

    public abstract class AbstractStarLibraryControllerList<M> : AbstractPointLibraryControllerList<M> where M : AbstractPointLibrary<M>, new()
    {
        public SourceOption StarOptions { get; private set; }
        public AbstractStarLibraryControllerList() : base()
        {
            StarOptions = new SourceOption(new RecordSource<Star>(DatabaseManager.Find<Star>()!), "PointName", OrderBy.ASC, "PointId");
        }

        public AbstractStarLibraryControllerList(TransitType transitType) : this() =>
        TransitType = transitType;

    }

    public class LibraryStarAspectsControllerList : AbstractStarLibraryControllerList<LibraryStarAspects>
    {
        public SourceOption StarOptions2 { get; private set; }
        public SourceOption EnergyOptions { get; private set; }

        public LibraryStarAspectsControllerList(TransitType transitType) : base(transitType)
        {
            StarOptions2 = new SourceOption(new RecordSource<Star>(DatabaseManager.Find<Star>()!), "PointName", OrderBy.ASC, "PointId");
            EnergyOptions = new SourceOption(new RecordSource<Energy>(DatabaseManager.Find<Energy>()!), "EnergyName");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            StarOptions.Conditions<WhereClause>(SearchQry, "PointId");
            StarOptions2.Conditions<WhereClause>(SearchQry, "StarID");
            EnergyOptions.Conditions<WhereClause>(SearchQry, "EnergyId");
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        protected override void Open(LibraryStarAspects model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryStarAspectWindow(model).ShowDialog();
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

    public class LibraryStarHousesControllerList : AbstractStarLibraryControllerList<LibraryStarHouses>
    {
        public SourceOption HouseOptions { get; private set; }
        public LibraryStarHousesControllerList(TransitType transitType) : base(transitType)
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

        protected override void Open(LibraryStarHouses model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryStarHouseWindow(model).ShowDialog();
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

    public class LibraryStarSignsControllerList : AbstractStarLibraryControllerList<LibraryStarSigns>
    {
        public SourceOption SignOptions { get; private set; }
        public LibraryStarSignsControllerList(TransitType transitType) : base(transitType)
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

        protected override void Open(LibraryStarSigns model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryStarSignWindow(model).ShowDialog();
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

    public class LibraryHouseSignsControllerList : AbstractStarLibraryControllerList<LibraryHouseSigns>
    {
        public SourceOption HouseOptions { get; private set; }
        public SourceOption SignOptions { get; private set; }

        public LibraryHouseSignsControllerList(TransitType transitType) : base(transitType)
        {
            SignOptions = new SourceOption(new RecordSource<Sign>(DatabaseManager.Find<Sign>()!), "SignName", OrderBy.ASC, "SignId");
            HouseOptions = new SourceOption(new RecordSource<House>(DatabaseManager.Find<House>()!), "PointName", OrderBy.ASC, "PointId");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            HouseOptions.Conditions<WhereClause>(SearchQry, "PointId");
            SignOptions.Conditions<WhereClause>(SearchQry, "SignId");
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override void SetTitle()
        {
            long transitID = TransitType.TransitTypeId;

            switch (transitID)
            {
                case 1:
                    ((Window?)UI).Title = "House in Sign";
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

        protected override void Open(LibraryHouseSigns model)
        {
            if (model.IsNewRecord())
            {
                model.TransitType = TransitType;
                model.Clean();
            }
            new LibraryHouseSignWindow(model).ShowDialog();
        }
    }
}