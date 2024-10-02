using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class SignWindow : Window
    {
        public SignWindow()
        {
            InitializeComponent();
            this.SetController(new SignController());
        }

        public SignWindow(Sign sign) : this()
        {
            this.GetController<SignController>()?.GoAt(sign);
        }
    }
}