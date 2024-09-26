using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;

namespace WpfApp1.View
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