using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryHousesWindowList : Window
    {
        public LibraryHousesWindowList()
        {
            InitializeComponent();
        }

        public LibraryHousesWindowList(TransitType transit) : this()
        {
            long transitID = transit.TransitTypeId;

            switch (transitID) 
            {
                case 1:
                    Title = "Radix Houses";
                    break;
                case 2:
                    Title = "Transits in Houses";
                    break;
            }
            this.SetController(new LibraryHousesControllerList(transit));
        }
    }
}