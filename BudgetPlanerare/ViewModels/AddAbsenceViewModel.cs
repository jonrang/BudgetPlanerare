using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BudgetPlanerare.Command;
using BudgetPlanerare.Models;
using BudgetPlanerare.Service;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class AddAbsenceViewModel : ViewModelBase
    {
        public Action<bool>? CloseAction { get; set; }

        public Absence CreatedAbsence { get; private set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsSickness { get; set; } = true; 
        public bool IsVAB { get; set; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AddAbsenceViewModel()
        {
            SaveCommand = new DelegateCommand(OnSave);
            CancelCommand = new DelegateCommand(_ => CloseAction?.Invoke(false));
        }

        private void OnSave(object? obj)
        {
            var ds = new DataService();
            if (ds.HasAbsenceOnDate(Date))
            {
                MessageBox.Show($"Det finns redan en frånvaro registrerad på {Date:yyyy-MM-dd}.\nTa bort den gamla först om du vill ändra.", "Upptaget datum");
                return;
            }

            CreatedAbsence = new Absence
            {
                Date = Date,
                Type = IsVAB ? AbsenceType.VAB : AbsenceType.Sickness
            };
            CloseAction?.Invoke(true);
        }
    }
}
