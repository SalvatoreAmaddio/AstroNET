using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryHouseWindow : Window
    {
        public LibraryHouseWindow()
        {
            InitializeComponent();
        }

        public LibraryHouseWindow(LibraryHouses record) : this()
        {
            this.SetController(new LibraryHousesController(record));
            this.GetController<LibraryHousesController>()?.SetTitle();
        }
    }
}
