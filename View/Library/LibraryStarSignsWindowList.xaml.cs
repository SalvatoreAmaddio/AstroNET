using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
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