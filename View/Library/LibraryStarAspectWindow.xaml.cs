using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryStarAspectWindow : Window
    {
        public LibraryStarAspectWindow()
        {
            InitializeComponent();
        }

        public LibraryStarAspectWindow(LibraryStarAspects record) : this()
        {
            this.SetController(new LibraryStarAspectsController(record));
            this.GetController<LibraryStarAspectsController>()?.SetTitle();
        }
    }
}
