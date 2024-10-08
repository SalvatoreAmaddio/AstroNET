using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;

namespace AstroNET.View
{
    public partial class SetDefaultCityWindow : Window
    {
        public SetDefaultCityWindow()
        {
            InitializeComponent();
            this.SetController(new CityListController());
            this.GetController<CityListController>().Search = AstroNETSettings.Default.DefaultCity;
        }

        private void OnLabelClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new AtlasDownloadForm().ShowDialog();
        }
    }
}
