using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class TransitsList : Window
    {
        public ObservableCollection<Group> Groups { get; set; }
        
        public TransitsList()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public TransitsList(IEnumerable<Aspect> aspects) : this() 
        {
            Groups = new(aspects.GroupBy(s => s.PointB).Select(s => new Group() { PointA = s.Key, Aspects = s.ToList() }).ToList());
        }
    }

    public class Group 
    { 
        public IPoint PointA { get; set; }
        public IEnumerable<Aspect> Aspects { get; set; }
    }

    public class R : List<Aspect> 
    { 
        public void reorder() 
        {
            List<Aspect> dummy = [];

            foreach (var aspect in this) 
            { 
                if (dummy.Count == 0) 
                {
                    dummy.Add(aspect);
                }
                else 
                {

                    dummy.Add(aspect);
                }
            }
        }
    }
}
