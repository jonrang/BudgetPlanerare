using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class TransactionItemViewModel : ViewModelBase, IHistoryItem
    {
        private readonly Transaction _model;

        public TransactionItemViewModel(Transaction model)
        {
            _model = model;
        }

        public Transaction GetModel() => _model;

        public string Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name != value)
                {
                    _model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get => _model.Amount;
            set
            {
                if (_model.Amount != value)
                {
                    _model.Amount = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(FormattedAmount));
                    RaisePropertyChanged(nameof(DisplayColor));
                }
            }
        }

        public DateTime Date
        {
            get => _model.Date;
            set
            {
                if (_model.Date != value)
                {
                    _model.Date = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DisplayDate));
                }
            }
        }

        public bool IsRepeating
        {
            get => _model.IsRepeating;
            set
            {
                if (_model.IsRepeating != value)
                {
                    _model.IsRepeating = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(RepeatingVisibility));
                }
            }
        }


        public bool IsIncome
        {
            get => _model.Category.IsIncome;
            set 
            {
                if (_model.Category.IsIncome != value)
                {
                    _model.Category.IsIncome = value;
                    RaisePropertyChanged();

                }
            }
        }


        public DateTime? EndDate
        {
            get => _model.EndDate;
            set
            {
                if (_model.EndDate != value)
                {
                    _model.EndDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Frequency Frequency
        {
            get => _model.Frequency;
            set
            {
                if (_model.Frequency != value)
                {
                    _model.Frequency = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(FrequencyName));
                }
            }
        }

        public int? YearlyOccurringMonth
        {
            get => _model.YearlyOccurringMonth;
            set
            {
                if (_model.YearlyOccurringMonth != value)
                {
                    _model.YearlyOccurringMonth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CategoryId
        {
            get => _model.CategoryId;
            set
            {
                if (_model.CategoryId != value)
                {
                    _model.CategoryId = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string DisplayDate => _model.Date.ToString("yyyy-MM-dd");
        public string CategoryName => _model.Category?.Name ?? "No Category";

        public string FrequencyName
        {
            get
            {
                switch (_model.Frequency)
                {
                    case Frequency.Monthly: return "Månadsvis";
                    case Frequency.Yearly: return "Årlig";
                    default: return "Engångs";
                }
            }
        }
        public string FormattedAmount
        {
            get
            {
                bool isIncome = _model.Category?.IsIncome ?? false;
                return isIncome ? $"+ {_model.Amount:N0} kr" : $"- {_model.Amount:N0} kr";
            }
        }

        public string DisplayColor
        {
            get
            {
                bool isIncome = _model.Category?.IsIncome ?? false;
                return isIncome ? "Green" : "#D63333"; 
            }
        }

        public string RepeatingVisibility => _model.IsRepeating ? "Visible" : "Collapsed";
        public string TypeName => FrequencyName;
        public bool IsTransaction => true;
        public int Id => _model.Id;
    }
}
