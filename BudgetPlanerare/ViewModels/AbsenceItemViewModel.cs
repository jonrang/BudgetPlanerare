using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class AbsenceItemViewModel : ViewModelBase, IHistoryItem
    {
        private readonly Absence _model;
        private readonly decimal _estimatedCost;

        public AbsenceItemViewModel(Absence model, decimal estimatedCost)
        {
            _model = model;
            _estimatedCost = estimatedCost;
        }

        public int Id => _model.Id;
       
        public DateTime Date
        {
            get => _model.Date;
            set
            {
                _model.Date = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayDate => _model.Date.ToString("yyyy-MM-dd");

        public string Name => "Löneavdrag"; 

        public string CategoryName => "Frånvaro";

        public string TypeName => _model.Type.ToString();

        public string FormattedAmount => $"-{_estimatedCost:N0} kr";

        public AbsenceType Type
        {
            get => _model.Type;
            set
            {
                _model.Type = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayColor));
            }
        }

        public string DisplayColor => _model.Type == AbsenceType.VAB ? "DodgerBlue" : "OrangeRed";
        public bool IsTransaction => false;
    }
}
