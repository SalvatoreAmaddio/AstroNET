using FrontEnd.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        public LibraryAspectWindow(LibraryAspect record) : this()
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
