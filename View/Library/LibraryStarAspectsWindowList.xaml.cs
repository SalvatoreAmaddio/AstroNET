using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
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