using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class LibraryStarAspectsWindowList : Window
    {
        public LibraryStarAspectsWindowList()
        {
            InitializeComponent();
        }

        public LibraryStarAspectsWindowList(TransitType transit) : this()
        {
            this.SetController(new LibraryStarAspectsControllerList(transit));
            this.GetController<LibraryStarAspectsControllerList>()?.SetTitle();
        }
    }
}