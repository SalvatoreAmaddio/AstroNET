using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryHouseSignWindow : Window
    {
        public LibraryHouseSignWindow()
        {
            InitializeComponent();
        }

        public LibraryHouseSignWindow(LibraryHouseSigns record) : this() 
        {
            this.SetController(new LibraryHouseSignsController(record));
            this.GetController<LibraryHouseSignsController>()?.SetTitle();
        }
    }
}
