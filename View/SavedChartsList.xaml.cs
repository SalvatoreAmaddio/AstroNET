using AstroNET.controller;
using AstroNET.model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace AstroNET.View
{
    public partial class SavedChartsList : Window
    {
        public SavedChartsList()
        {
            InitializeComponent();
            this.SetController(new SavedChartControllerList());
        }

        public SavedChartsList(int skyTypeId, SkyEvent skyEvent)
        {
            InitializeComponent();
            this.SetController(new SavedChartControllerList(skyTypeId, skyEvent));
        }
    }
}