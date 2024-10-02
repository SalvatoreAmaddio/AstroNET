using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;

namespace AstroNET.View
{

    public partial class LibraryHouseSignsWindowList : Window
    {
        public LibraryHouseSignsWindowList()
        {
            InitializeComponent();
            this.SetController(new LibraryHouseSignsControllerList(new(1)));
            this.GetController<LibraryHouseSignsControllerList>()?.SetTitle();
        }
    }
}