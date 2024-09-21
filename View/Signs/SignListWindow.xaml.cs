using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
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
