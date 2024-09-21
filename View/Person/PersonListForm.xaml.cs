using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
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
            this.GetController<PersonListController>()!.Sky1 = Owner?.GetSky();
        }
    }
}
