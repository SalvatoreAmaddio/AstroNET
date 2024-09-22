using Backend.Database;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Source;
using WpfApp1.model;
using Backend.ExtensionMethods;
using System.Windows;

namespace WpfApp1.controller
{
    public abstract class AbstractLibraryController<M> : AbstractFormController<M> where M : AbstractLibrary<M>, new()
    {
        private M? _aspect;
        protected TransitType? TransitType { get; set; }
        public RecordSource<Star> Stars { get; private set; } = new(DatabaseManager.Find<Star>()!);
        public AbstractLibraryController()
        {
            WindowLoaded += OnWindowLoaded;
            AfterRecordNavigation += OnAfterRecordNavigation;
        }

        private void OnAfterRecordNavigation(object? sender, Backend.Events.AllowRecordMovementArgs e)
        {
            if (e.NewRecord)
            {
                CurrentRecord.TransitType = TransitType;
                CurrentRecord.Clean();
            }
        }

        public AbstractLibraryController(M aspect) : this()
        {
            this.TransitType = aspect.TransitType;
            _aspect = aspect;
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new M().
                Select().All()
                .From()
                .Where()
                    .EqualsTo("TransitTypeId", "@transitId");
        }

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            SearchQry.AddParameter("transitId", TransitType?.TransitTypeId);
            RecordSource<M> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            RecordSource.ReplaceRecords(results);
            GoAt(_aspect);
        }

        public abstract void SetTitle();
    }

    public class LibraryAspectsController : AbstractLibraryController<LibraryAspects>
    {
        public RecordSource<Energy> Energies { get; private set; } = new(DatabaseManager.Find<Energy>()!);
        public RecordSource<Star> Stars2 { get; private set; } = new(DatabaseManager.Find<Star>()!);

        public LibraryAspectsController(LibraryAspects aspect) : base(aspect)
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

    public class LibraryHousesController : AbstractLibraryController<LibraryHouses> 
    {
        public RecordSource<House> Houses { get; private set; } = new(DatabaseManager.Find<House>()!);

        public LibraryHousesController(LibraryHouses aspect) : base(aspect)
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

}
