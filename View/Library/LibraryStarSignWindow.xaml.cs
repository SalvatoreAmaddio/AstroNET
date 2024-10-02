using FrontEnd.ExtensionMethods;
using System.Windows;
using AstroNET.controller;
using AstroNET.model;

namespace AstroNET.View
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
