using System.Windows;
using AstroNET.controller;
using FrontEnd.ExtensionMethods;

namespace AstroNET.View
{
    public partial class PointListWindow : Window
    {
        public PointListWindow()
        {
            InitializeComponent();
        }

        public PointListWindow(bool isStar) : this() 
        {
            if (isStar)
                this.SetController(new StarListController());
            else 
            {
                Title = "Houses";
                this.SetController(new HouseListController());
            }
        }
    }
}