using MvvmHelpers;
using System.ComponentModel;
using System.Windows;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class TransitsList : Window, INotifyPropertyChanged
    {
        private bool _starGrouping = true;
        private bool _dateGrouping = false;
        private IEnumerable<Aspect> _aspects;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableRangeCollection<TransitGroup> Groups { get; set; } = null!;

        public bool StarGrouping 
        {
            get => _starGrouping;
            set 
            {
                _starGrouping = value;
                OnPropertyChanged(nameof(StarGrouping));
                if (value) 
                {
                    GroupByStar();
                    OnPropertyChanged(nameof(Groups)); // Notify WPF of the change
                }
            }
        }

        public bool DateGrouping
        {
            get => _dateGrouping;
            set
            {
                _dateGrouping = value;
                OnPropertyChanged(nameof(DateGrouping));
                if (value) 
                {
                    GroupByDate();
                    OnPropertyChanged(nameof(Groups)); // Notify WPF of the change
                }
            }
        }
        
        public TransitsList()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public TransitsList(IEnumerable<Aspect> aspects) : this() 
        {
            _aspects = aspects;
            GroupByStar();
        }

        private void GroupByStar()
        {
            Groups = new(
                    _aspects.GroupBy(s => s.PointB.PointName)
                           .Select(s => new TransitGroup()
                           {
                               Header = s.Key,
                               //Aspects = s.ToList()
                               Aspects = Filter(s.ToList())
                           }).ToList());
        }

        private void GroupByDate()
        {
            List<TransitGroup> x = _aspects.GroupBy(s => s.DateOf)
                           .Select(s => new TransitGroup()
                           {
                               TransitDate = s.Key,
                               Header = s.Key.ToString("dd/MM/yyyy"),
                               Aspects = Filter(s.ToList())
                           }).ToList();

            var a = x.GroupBy(s => new { s.TransitDate.Month, s.TransitDate.Year }).ToList();

            Groups = new(
                    _aspects.GroupBy(s => s.DateOf)
                           .Select(s => new TransitGroup()
                           {
                               TransitDate = s.Key,
                               Header = s.Key.ToString("dd/MM/yyyy"),
                               Aspects = s.ToList()
                           }).ToList());
        }

        private List<Aspect> Filter(List<Aspect> aspects) 
        {
            List<Aspect> filteredAspects = [];
            List<Aspect> Copy = new(aspects);

            for (int i = 0; i <= aspects.Count; i++) 
            {
                var subChunkToFilter = Copy.TakeWhile(s=>s.AspectId == aspects[i].AspectId && s.PointAR() == aspects[i].PointAR()).ToList();
                i = subChunkToFilter.Count -1;
                var toAdd = Pick(subChunkToFilter);
                filteredAspects.AddRange(toAdd);
                Copy.RemoveRange(0, subChunkToFilter.Count);
                if (Copy.Count == 0) break;
            }

            return filteredAspects;
        }

        private static List<Aspect> Pick(List<Aspect> chunk) 
        {
            List<Aspect> r = [];
            Aspect? a;
            Aspect? b;
            Aspect? c;
            Aspect? d;

            a = chunk.FirstOrDefault();
            if (a != null)
                r.Add(a);

            b = chunk.Where(s=>s.OrbDiff>0).LastOrDefault();
            if (b!= null && !r.Any(s=>s.IsSame(b)))
                r.Add(b);
            
            c = chunk.Where(s => s.OrbDiff < 0).FirstOrDefault();            
            if (c != null && !r.Any(s => s.IsSame(c)))
                r.Add(c);

            d = chunk.LastOrDefault();    
            if (d != null && !r.Any(s => s.IsSame(d)))
                r.Add(d);

            return r;
        }

        public void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new(propName));
    }

    public class TransitGroup 
    { 
        public DateTime TransitDate { get; set; }
        public string Header { get; set; } = string.Empty;
        public IEnumerable<Aspect> Aspects { get; set; } = null!;
    }
}