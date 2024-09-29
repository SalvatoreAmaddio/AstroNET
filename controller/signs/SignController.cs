using Backend.Database;
using FrontEnd.Controller;
using FrontEnd.Source;
using WpfApp1.model;
using FrontEnd.Events;
using WpfApp1.View;

namespace WpfApp1.controller
{
    public class SignController : AbstractFormController<Sign>
    {
        private Star? _selectedRuller;
        private StarListController _starListController = new();

        public Star? SelectedRuller 
        {
            get => _selectedRuller;
            set => UpdateProperty(ref value, ref _selectedRuller);
        }

        public RecordSource<Star> Rullers { get; private set; } = [];

        public SignController() 
        {
            AllowNewRecord = false;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(CurrentRecord))) 
            {
                Rullers.ReplaceRecords(DatabaseManager.Find<StarPower>()?
                .MasterSource.Cast<StarPower>()
                .Where(s => s.Power.PowerID == 1 && s.Sign.SignId == this.CurrentRecord.SignId).Select(s => s.Star));
                SelectedRuller = null;
            }

            if (e.Is(nameof(SelectedRuller))) 
            { 
                if (SelectedRuller != null) 
                {
                    new StarWindow(_starListController.RecordSource.FirstOrDefault(s => s.Equals(SelectedRuller))).ShowDialog();
                }           
            }
        }
    }
}