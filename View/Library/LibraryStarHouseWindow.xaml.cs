using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryStarHouseWindow : Window
    {
        public LibraryStarHouseWindow()
        {
            InitializeComponent();
        }

        public LibraryStarHouseWindow(LibraryStarHouses record) : this()
        {
            this.SetController(new LibraryStarHousesController(record));
            this.GetController<LibraryStarHousesController>()?.SetTitle();
        }
    }
}
