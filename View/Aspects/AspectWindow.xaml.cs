using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;
using System.Windows.Input;

namespace AstroNET.View
{
    public partial class AspectWindow : Window
    {
        public AspectWindow()
        {
            InitializeComponent();
            this.SetController(new AspectController());
        }

        public AspectWindow(Aspect aspect) : this() 
        {
            this.GetController<AspectController>()?.GoAt(aspect);        
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new StarTransitOrbitWindowList(this.GetController<AspectController>().CurrentRecord).ShowDialog();
        }
    }
}