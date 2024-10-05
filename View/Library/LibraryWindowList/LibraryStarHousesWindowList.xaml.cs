using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class LibraryStarHousesWindowList : Window
    {
        public LibraryStarHousesWindowList()
        {
            InitializeComponent();
        }

        public LibraryStarHousesWindowList(TransitType transit) : this()
        {
            this.SetController(new LibraryStarHousesControllerList(transit));
            this.GetController<LibraryStarHousesControllerList>()?.SetTitle();
        }
    }
}