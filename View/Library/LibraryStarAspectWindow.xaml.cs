using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
