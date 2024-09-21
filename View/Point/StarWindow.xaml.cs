using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class StarWindow : Window
    {
        public StarWindow()
        {
            InitializeComponent();
            this.SetController(new StarController());
        }

        public StarWindow(Star star) : this()
        {
            this.GetController<StarController>()?.GoAt(star);              
        }
    }
}
