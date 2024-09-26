using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryStarSignWindow : Window
    {
        public LibraryStarSignWindow()
        {
            InitializeComponent();
        }

        public LibraryStarSignWindow(LibraryStarSigns record) : this() 
        {
            this.SetController(new LibraryStarSignsController(record));
            this.GetController<LibraryStarSignsController>()?.SetTitle();
        }
    }
}
