using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.Windows.Input;
using AstroNET.model;
using AstroNET.View;

namespace AstroNET.controller
{
    public class PersonListController : AbstractFormListController<Person>
    {
        public SkyEvent? Sky1;
        public ICommand CalculateChartCMD { get; }
        public PersonListController()
        {
            CheckIsDirtyOnClose = false;
            CalculateChartCMD = new CMDAsync(CalculateChart);
            AfterUpdate += OnAfterUpdate;
        }
        
        private async Task CalculateChart() 
        {
            CurrentRecord?.City?.Build();

            SkyEvent sky = new(CurrentRecord!, this);

            if (Sky1 != null) 
            {
                IsLoading = true;

                SinastryBundle sinastryBundle = await Sky1.CalculateSinastryAsync(sky);

                IsLoading = false;
                ChartOpener.OpenComparedChart(sinastryBundle);
            }
            else ChartOpener.OpenChart($"{CurrentRecord}", sky, sky.SkyInfo.SkyType);
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new Person()
            .Select().All()
            .From()
            .Where()
                .OpenBracket()
                    .Like("LOWER(FirstName)", "@name")
                    .OR()
                    .Like("LOWER(LastName)", "@name")
                .CloseBracket();
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<Person>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Person model)
        {
        }
    }
}