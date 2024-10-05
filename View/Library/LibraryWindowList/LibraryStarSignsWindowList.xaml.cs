using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;

namespace AstroNET.View
{

    public partial class LibraryStarSignsWindowList : Window
    {
        public LibraryStarSignsWindowList()
        {
            InitializeComponent();
            this.SetController(new LibraryStarSignsControllerList(new(1)));
            this.GetController<LibraryStarSignsControllerList>()?.SetTitle();
        }
    }
}