using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.Windows.Input;
using WpfApp1.model;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class PersonListController : AbstractFormListController<Person>
    {
        public SkyEvent? Sky1;
        public ICommand CalculateChartCMD { get; }
        public PersonListController()
        {
            CheckIsDirtyOnClose = false;
            CalculateChartCMD = new CMD(CalculateChart);
            AfterUpdate += OnAfterUpdate;
        }
        
        private void CalculateChart() 
        {
            CurrentRecord?.City?.Build();

            SkyEvent sky = new(CurrentRecord!, this);

            if (Sky1 != null) 
            {
                ChartOpener.OpenComparedChart($"{Sky1.Person} AND {CurrentRecord}", Sky1, sky, SkyType.Sinastry);
            }
            else ChartOpener.OpenChart($"{CurrentRecord}", sky, sky.SkyType.ToString());
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