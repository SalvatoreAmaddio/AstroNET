using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class StarTransitOrbitWindowList : Window
    {
        public StarTransitOrbitWindowList()
        {
            InitializeComponent();
            this.SetController(new StarTransitOrbitListController());
        }

        public StarTransitOrbitWindowList(Aspect aspect) : this() 
        {
            Title = $"{aspect.AspectName}'s Transit Orbit per Star";
            this.GetController<StarTransitOrbitListController>()!.Aspect = aspect;
        }
    }
}
