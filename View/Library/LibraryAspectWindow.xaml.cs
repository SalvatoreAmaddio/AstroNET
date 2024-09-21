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

        public LibraryAspectWindow(LibraryAspects record) : this()
        {
            long transitId = record.TransitType.TransitTypeId;

            switch (transitId) 
            {
                case 1:
                    Title = "Radix Aspect";
                    break;
                case 2:
                    Title = "Transit Aspect";
                    break;
            }

            this.SetController(new LibraryAspectsController(record));
        }
    }
}
