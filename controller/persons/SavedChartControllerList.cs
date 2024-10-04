using AstroNET.model;
using AstroNET.View;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Primitives;

namespace AstroNET.controller
{
    public class SavedChartControllerList : AbstractFormListController<SavedCharts>
    {
        private SkyEvent subjectSky = null!;
        private long PersonId;
        private int SkyTypeId;
        public override AbstractClause InstantiateSearchQry() =>
        new SavedCharts()
            .Select().All()
            .From()
            .Where()
                .EqualsTo("PersonId", "@personId")
                .AND()
                .EqualsTo("SkyTypeId", "@skyTypeId")
            .OrderBy()
                .Field("DateOf DESC").Field("TimeOf DESC");

        public SavedChartControllerList()
        {
            AllowNewRecord = false;
        }

        public SavedChartControllerList(int skyTypeId, SkyEvent subjectSky) : this()
        {
            this.subjectSky = subjectSky;
            this.PersonId = this.subjectSky.Person.PersonId;
            this.SkyTypeId = skyTypeId;
            WindowLoaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            await OnSearchPropertyRequeryAsync(this);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<SavedCharts>> SearchRecordAsync()
        {
            SearchQry.AddParameter("personId", PersonId);
            SearchQry.AddParameter("skyTypeId", SkyTypeId);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(SavedCharts model)
        {
            model.City.Build();
            switch (SkyTypeId)
            {
                case 2:
                    SkyEvent cloneSky = subjectSky.CloneMe();
                    cloneSky.CalculateHoroscope(model.Dateof, model.Timeof, model.City);
                    ChartOpener.OpenChart($"{subjectSky?.Person?.ToString()}", cloneSky!, SkyType.Horoscope);
                    break;
                case 4:
                case 5:
                    ReturnSkyEvent returnSky = subjectSky.CalculateReturn(model.Dateof, model.Timeof, model.City, (SkyTypeId == 4) ? SkyType.SunReturn : SkyType.MoonReturn);
                    ChartOpener.OpenComparedChart($"{subjectSky.Person}", subjectSky, returnSky, returnSky.SkyInfo.SkyType);
                    break;
            }
        }
    }
}
