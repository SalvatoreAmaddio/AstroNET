using FrontEnd.ExtensionMethods;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
            Icon = new BitmapImage(new Uri(aspect.URI, UriKind.RelativeOrAbsolute));
            this.GetController<StarTransitOrbitListController>()!.Aspect = aspect;
        }
    }
}
