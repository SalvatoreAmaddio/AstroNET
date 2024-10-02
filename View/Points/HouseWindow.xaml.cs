using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
