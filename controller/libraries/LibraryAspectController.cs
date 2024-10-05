using Backend.Database;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Source;
using AstroNET.model;
using Backend.ExtensionMethods;
using System.Windows;
using System.Windows.Input;

namespace AstroNET.controller
{
    public abstract class AbstractPointLibraryController<M> : AbstractFormController<M> where M : AbstractPointLibrary<M>, new()
    {
        public ICommand OpenInfoWindowCMD { get; }
        private M? _libraryRecord;
        protected TransitType? TransitType { get; set; }

        public AbstractPointLibraryController()
        {
            WindowLoaded += OnWindowLoaded;
            AfterRecordNavigation += OnAfterRecordNavigation;
            OpenInfoWindowCMD = new CMD(OpenInfoWindow);
        }

        public AbstractPointLibraryController(M libraryRecord) : this()
        {
            this.TransitType = libraryRecord.TransitType;
            _libraryRecord = libraryRecord;
        }

        protected abstract void OpenInfoWindow();

        private void OnAfterRecordNavigation(object? sender, Backend.Events.AllowRecordMovementArgs e)
        {
            if (e.NewRecord)
            {
                CurrentRecord.TransitType = TransitType;
                CurrentRecord.Clean();
            }
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new M().
                Select().All()
                .From()
                .Where()
                    .EqualsTo("TransitTypeId", "@transitId");
        }

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("transitId", TransitType?.TransitTypeId);
            RecordSource<M> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            RecordSource.ReplaceRecords(results);
            GoAt(_libraryRecord);
        }

        public abstract void SetTitle();
    }

    public abstract class AbstractStarLibraryController<M> : AbstractPointLibraryController<M> where M : AbstractStarLibrary<M>, new()
    {
        public RecordSource<Star> Stars { get; private set; } = new(DatabaseManager.Find<Star>()!);
        public AbstractStarLibraryController() : base()
        {
        }

        public AbstractStarLibraryController(M libraryRecord) : base(libraryRecord)
        {
        }

        protected override void OpenInfoWindow()
        {
            MessageBox.Show($"{CurrentRecord.Star}");
        }
    }

    public class LibraryStarAspectsController : AbstractStarLibraryController<LibraryStarAspects>
    {
        public RecordSource<Energy> Energies { get; private set; } = new(DatabaseManager.Find<Energy>()!);
        public RecordSource<Star> Stars2 { get; private set; } = new(DatabaseManager.Find<Star>()!);

        public LibraryStarAspectsController(LibraryStarAspects libraryRecord) : base(libraryRecord)
        {
        }

        public override void SetTitle()
        {
            long transitId = TransitType.TransitTypeId;

            switch (transitId)
            {
                case 1:
                    ((Window?)UI).Title = "Radix Aspect";
                    break;
                case 2:
                    ((Window?)UI).Title = "Transit Aspect";
                    break;
                case 3:
                    ((Window?)UI).Title = "Sinastry Aspects";
                    break;
            }
        }
    }

    public class LibraryStarHousesController : AbstractStarLibraryController<LibraryStarHouses>
    {
        public RecordSource<House> Houses { get; private set; } = new(DatabaseManager.Find<House>()!);

        public LibraryStarHousesController(LibraryStarHouses libraryRecord) : base(libraryRecord)
        {
        }

        public override void SetTitle()
        {
            long transitId = TransitType.TransitTypeId;

            switch (transitId)
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

    public class LibraryStarSignsController : AbstractStarLibraryController<LibraryStarSigns>
    {
        public RecordSource<Sign> Signs { get; private set; } = new(DatabaseManager.Find<Sign>()!);

        public LibraryStarSignsController(LibraryStarSigns libraryRecord) : base(libraryRecord)
        {
        }

        public override void SetTitle()
        {
            long transitId = TransitType.TransitTypeId;

            switch (transitId)
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

    public class LibraryHouseSignsController : AbstractPointLibraryController<LibraryHouseSigns>
    {
        public RecordSource<House> Houses { get; private set; } = new(DatabaseManager.Find<House>()!);
        public RecordSource<Sign> Signs { get; private set; } = new(DatabaseManager.Find<Sign>()!);

        public LibraryHouseSignsController(LibraryHouseSigns libraryRecord) : base(libraryRecord)
        {
        }

        protected override void OpenInfoWindow()
        {
            MessageBox.Show("");
        }

        public override void SetTitle()
        {
            long transitId = TransitType.TransitTypeId;

            switch (transitId)
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
    }

}
