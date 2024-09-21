using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class HouseWindow : Window
    {
        public HouseWindow()
        {
            InitializeComponent();
            this.SetController(new HouseController());
        }

        public HouseWindow(House house) : this()
        {
            this.GetController<HouseController>()?.GoAt(house);
        }
    }
}
