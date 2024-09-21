using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
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