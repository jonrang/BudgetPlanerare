using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class AbsenceItemViewModel : ViewModelBase
    {
        private readonly Absence _model;

        public AbsenceItemViewModel(Absence model)
        {
            _model = model;
        }

        public DateTime Date
        {
            get => _model.Date;
            set
            {
                _model.Date = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayDate => _model.Date.ToString("yyyy-MM-dd (dddd)");

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
    }
}
