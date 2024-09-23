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
                               Aspects = s.ToList()
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

        private bool x(Aspect prev, Aspect current) 
        {
            bool prevR = prev.PointA is Star star1 && star1.IsRetrograde;
            bool currentR = current.PointA is Star star2 && star2.IsRetrograde;
            var a = prev.FullInfo;
            var b = current.FullInfo;

            if (prevR != currentR) return true;

            if (prev.OrbDiff > current.OrbDiff) 
            {
                var diff = prev.OrbDiff - current.OrbDiff;
                return diff <= 0;
            }

            return false;
        }

        private List<Aspect> Filter(List<Aspect> aspects) 
        {
            List<Aspect> filteredAspects = [];

            int i = 0;
            do
            {
                if (i == 0) 
                {
                    filteredAspects.Add(aspects[i]);
                    i++;
                    continue;
                }

                var prev = filteredAspects[filteredAspects.Count-1];
                var current = aspects[i];

                //if (x(prev, current))
                    filteredAspects.Add(current);
    
                i++;
            }
            while (i < aspects.Count);
            return filteredAspects;
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