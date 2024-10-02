using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class LibraryHouseSignWindow : Window
    {
        public LibraryHouseSignWindow()
        {
            InitializeComponent();
        }

        public LibraryHouseSignWindow(LibraryHouseSigns record) : this() 
        {
            this.SetController(new LibraryHouseSignsController(record));
            this.GetController<LibraryHouseSignsController>()?.SetTitle();
        }
    }
}
