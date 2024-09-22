using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryAspectsWindowList : Window
    {
        public LibraryAspectsWindowList()
        {
            InitializeComponent();
        }

        public LibraryAspectsWindowList(TransitType transit) : this()
        {
            this.SetController(new LibraryAspectsControllerList(transit));
            this.GetController<LibraryAspectsControllerList>()?.SetTitle();
        }
    }
}