using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class PersonListForm : Window
    {
        public PersonListForm()
        {
            InitializeComponent();
            this.SetController(new PersonListController());
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            Loaded -= OnLoaded;
            Closed -= OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.GetController<PersonListController>()!.Sky1 = (SkyEvent?)(Owner?.GetSky());
        }
    }
}
