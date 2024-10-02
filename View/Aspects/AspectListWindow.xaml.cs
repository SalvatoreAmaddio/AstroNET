using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;

namespace AstroNET.View
{
    public partial class AspectListWindow : Window
    {
        public AspectListWindow()
        {
            InitializeComponent();
            this.SetController(new AspectListController());
        }
    }
}