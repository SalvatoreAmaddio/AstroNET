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

        public TransitsList(List<TransitGroup> aspects) : this()
        {
            Groups = new(aspects);
            GroupByStar();
        }

        private void GroupByStar()
        {
        }

        private void GroupByDate()
        {
            //List<TransitGroup> x = _aspects.GroupBy(s => s.DateOf)
            //               .Select(s => new TransitGroup()
            //               {
            //                   TransitDate = s.Key,
            //                   Header = s.Key.ToString("dd/MM/yyyy"),
            //                   Aspects = Filter(s.ToList())
            //               }).ToList();

            //var a = x.GroupBy(s => new { s.TransitDate.Month, s.TransitDate.Year }).ToList();

            //Groups = new(
            //        _aspects.GroupBy(s => s.DateOf)
            //               .Select(s => new TransitGroup()
            //               {
            //                   TransitDate = s.Key,
            //                   Header = s.Key.ToString("dd/MM/yyyy"),
            //                   Aspects = s.ToList()
            //               }).ToList());
        }

        public void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new(propName));
    }
}