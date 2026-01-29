using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using BudgetPlanerare.Command;
using BudgetPlanerare.Models;
using BudgetPlanerare.Service;
using BudgetPlanerare.Services;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    internal class AddTransactionViewModel : ViewModelBase
    {
        private readonly DataService _dataService;

        public Transaction? CreatedTransaction { get; private set; }

        public Action<bool>? CloseAction { get; set; }

        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public ObservableCollection<Category> Categories { get; set; }
        public Category? SelectedCategory { get; set; }

        public ObservableCollection<Frequency> Frequencies { get; }
            = new(Enum.GetValues<Frequency>());
        public Frequency SelectedFrequency { get; set; } = Frequency.OneTime;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AddTransactionViewModel()
        {
            _dataService = new DataService();

            var categoriesFromDb = _dataService.GetCategories();
            Categories = new ObservableCollection<Category>(categoriesFromDb);
            
            SelectedCategory = Categories.FirstOrDefault();

            SaveCommand = new DelegateCommand(OnSave);
            CancelCommand = new DelegateCommand(_ => CloseAction?.Invoke(false));
        }

        private void OnSave(object? obj)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Du måste ge transaktionen ett namn.");
                return;
            }
            if (SelectedCategory == null)
            {
                MessageBox.Show("Välj en kategori.");
                return;
            }

            CreatedTransaction = new Transaction
            {
                Name = Name,
                Amount = Amount,
                Date = Date,
                CategoryId = SelectedCategory.Id,
                Category = SelectedCategory,
                Frequency = SelectedFrequency,
                IsRepeating = SelectedFrequency != Frequency.OneTime
            };

            CloseAction?.Invoke(true);
        }
    }
}
