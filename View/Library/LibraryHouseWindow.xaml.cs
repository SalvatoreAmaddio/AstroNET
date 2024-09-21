using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.controller;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class LibraryHouseWindow : Window
    {
        public LibraryHouseWindow()
        {
            InitializeComponent();
        }

        public LibraryHouseWindow(LibraryHouses record) : this()
        {
            long transitId = record.TransitType.TransitTypeId;

            switch (transitId) 
            {
                case 1:
                    Title = "Radix House";
                    break;
                case 2:
                    Title = "Transit House";
                    break;
            }

            this.SetController(new LibraryHousesController(record));
        }
    }
}
