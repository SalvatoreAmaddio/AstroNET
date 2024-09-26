using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryAspectWindow : Window
    {
        public LibraryAspectWindow()
        {
            InitializeComponent();
        }

        public LibraryAspectWindow(LibraryStarAspects record) : this()
        {
            this.SetController(new LibraryAspectsController(record));
            this.GetController<LibraryAspectsController>()?.SetTitle();
        }
    }
}
