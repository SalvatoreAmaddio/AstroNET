using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
