using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryHousesWindowList : Window
    {
        public LibraryHousesWindowList()
        {
            InitializeComponent();
        }

        public LibraryHousesWindowList(TransitType transit) : this()
        {
            this.SetController(new LibraryHousesControllerList(transit));
            this.GetController<LibraryHousesControllerList>()?.SetTitle();
        }
    }
}