using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using BudgetPlanerare.Command;
using BudgetPlanerare.Models;
using BudgetPlanerare.Service;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly UserProfile _model;

        public SettingsViewModel()
        {
            _dataService = new DataService();
            _model = _dataService.GetUserProfile();
            SaveCommand = new DelegateCommand(OnSave);
        }

        public decimal YearlyIncome
        {
            get => _model.YearlyIncome;
            set
            {
                if (_model.YearlyIncome != value)
                {
                    _model.YearlyIncome = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(HourlyRateDisplay));
                }
            }
        }

        public int YearlyWorkHours
        {
            get => _model.YearlyWorkHours;
            set
            {
                if (_model.YearlyWorkHours != value)
                {
                    _model.YearlyWorkHours = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(HourlyRateDisplay));
                }
            }
        }

        public string HourlyRateDisplay
        {
            get
            {
                if (YearlyWorkHours == 0) return "0 kr/h";
                return $"{Math.Round(YearlyIncome / YearlyWorkHours, 2)} kr/h";
            }
        }

        public DelegateCommand SaveCommand { get; }

        private void OnSave(object? obj)
        {
            _dataService.SaveUserProfile(_model);
            MessageBox.Show("Inställningar sparade!", "Klart");
        }
    }
}
