using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibrarySignWindow : Window
    {
        public LibrarySignWindow()
        {
            InitializeComponent();
        }

        public LibrarySignWindow(LibraryStarSigns record) : this() 
        {
            this.SetController(new LibrarySignsController(record));
            this.GetController<LibrarySignsController>()?.SetTitle();
        }
    }
}
