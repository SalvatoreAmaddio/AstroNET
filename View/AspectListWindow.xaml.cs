using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
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