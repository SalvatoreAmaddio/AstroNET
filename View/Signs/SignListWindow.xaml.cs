using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;

namespace AstroNET.View
{
    public partial class SignListWindow : Window
    {
        public SignListWindow()
        {
            InitializeComponent();
            this.SetController(new SignListController());
        }
    }
}
