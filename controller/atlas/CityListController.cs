using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using AstroNET.model;
using Backend.ExtensionMethods;
using System.Windows.Input;
using FrontEnd.Dialogs;
using System.Windows;

namespace AstroNET.controller
{
    public class CityListController : AbstractFormListController<City>
    {
        public ICommand SetDefaultCityCMD { get; }
        public CityListController()
        {
            AfterUpdate += OnAfterUpdate;
            AllowNewRecord = false;
            SetDefaultCityCMD = new CMD(SetDefaultCity);
        }

        private void SetDefaultCity()
        {
            AstroNETSettings.Default.DefaultCity = CurrentRecord?.CityName;
            AstroNETSettings.Default.Save();
            SuccessDialog.Display("New default city successfully saved");
            ((Window?)UI)?.Close();
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new City().Select().All().From().Where().Like("LOWER(CityName)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<City>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(City model)
        {
        }
    }
}