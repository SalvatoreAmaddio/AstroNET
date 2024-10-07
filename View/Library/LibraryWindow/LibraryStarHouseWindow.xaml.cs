using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
