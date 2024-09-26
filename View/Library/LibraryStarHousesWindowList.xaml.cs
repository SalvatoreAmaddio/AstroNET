using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
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