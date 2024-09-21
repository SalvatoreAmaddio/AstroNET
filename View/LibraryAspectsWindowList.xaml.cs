using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryAspectsWindowList : Window
    {
        public LibraryAspectsWindowList()
        {
            InitializeComponent();
        }

        public LibraryAspectsWindowList(TransitType transit) : this()
        {
            long transitID = transit.TransitTypeId;

            switch (transitID) 
            {
                case 1:
                    Title = "Radix Aspects";
                    break;
                case 2:
                    Title = "Transits Aspects";
                    break;
            }
            this.SetController(new LibraryAspectsControllerList(transit));
        }
    }
}