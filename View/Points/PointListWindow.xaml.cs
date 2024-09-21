using System.Windows;
using WpfApp1.controller;
using FrontEnd.ExtensionMethods;

namespace WpfApp1.View
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