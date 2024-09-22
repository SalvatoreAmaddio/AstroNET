using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
{

    public partial class LibrarySignsWindowList : Window
    {
        public LibrarySignsWindowList()
        {
            InitializeComponent();
            this.SetController(new LibrarySignsControllerList(new(1)));
            this.GetController<LibrarySignsControllerList>()?.SetTitle();
        }
    }
}