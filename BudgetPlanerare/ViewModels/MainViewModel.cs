using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using BudgetPlanerare.Command;
using BudgetPlanerare.Models;
using BudgetPlanerare.Service;
using BudgetPlanerare.Services;
using BudgetPlanerare.Views;
using BudgetPlanerare.VM;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BudgetPlanerare.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly CalculationService _calcService;


        private DateTime _currentViewDate;
        public DateTime CurrentViewDate
        {
            get => _currentViewDate;
            set
            {
                if (SetProperty(ref _currentViewDate, value))
                {
                    LoadData();
                }
            }
        }

        public SettingsViewModel SettingsVM { get; } = new SettingsViewModel();

        public string CurrentMonthHeader => _currentViewDate.ToString("MMMM yyyy").ToUpper();

        public ObservableCollection<TransactionItemViewModel> Transactions { get; set; } = new();

        private decimal _totalIncome;
        public decimal TotalIncome { get => _totalIncome; set => SetProperty(ref _totalIncome, value); }

        private decimal _totalExpense;
        public decimal TotalExpense { get => _totalExpense; set => SetProperty(ref _totalExpense, value); }

        private decimal _forecastBalance;
        public decimal ForecastBalance { get => _forecastBalance; set => SetProperty(ref _forecastBalance, value); }

        private bool _isDashboardVisible = true;
        public bool IsDashboardVisible { get => _isDashboardVisible; set => SetProperty(ref _isDashboardVisible, value); }

        private bool _isSettingsVisible = false;
        public bool IsSettingsVisible { get => _isSettingsVisible; set => SetProperty(ref _isSettingsVisible, value); }

        private string _salaryInfo;
        public string SalaryInfo { get => _salaryInfo; set => SetProperty(ref _salaryInfo, value); }

        private decimal _totalBalance;
        public decimal TotalBalance { get => _totalBalance; set => SetProperty(ref _totalBalance, value); }

        public DelegateCommand AddTransactionCommand { get; }
        public DelegateCommand DeleteTransactionCommand { get; }
        public DelegateCommand NextMonthCommand { get; }
        public DelegateCommand PreviousMonthCommand { get; }
        public DelegateCommand ShowDashboardCommand { get; }
        public DelegateCommand ShowSettingsCommand { get; }
        public DelegateCommand AddAbsenceCommand { get; }

        public MainViewModel()
        {
            _dataService = new DataService();
            _calcService = new CalculationService();

            _currentViewDate = DateTime.Now;

            AddTransactionCommand = new DelegateCommand(OnAddTransaction);
            DeleteTransactionCommand = new DelegateCommand(OnDeleteTransaction);

            NextMonthCommand = new DelegateCommand(_ => CurrentViewDate = CurrentViewDate.AddMonths(1));
            PreviousMonthCommand = new DelegateCommand(_ => CurrentViewDate = CurrentViewDate.AddMonths(-1));

            ShowDashboardCommand = new DelegateCommand(_ => SwitchView("dashboard"));
            ShowSettingsCommand = new DelegateCommand(_ => SwitchView("settings"));

            AddAbsenceCommand = new DelegateCommand(OnAddAbsence);

            LoadData();
        }


        private void LoadData()
        {
            RaisePropertyChanged(nameof(CurrentMonthHeader));

            Transactions.Clear();

            var rawData = _dataService.GetTransactions();

            foreach (var t in rawData)
            {
                Transactions.Add(new TransactionItemViewModel(t));
            }

            UpdateCalculations();
        }

        private void UpdateCalculations()
        {
            var profile = _dataService.GetUserProfile();

            var absences = _dataService.GetAbsencesForMonth(CurrentViewDate.Year, CurrentViewDate.Month);

            var netSalary = _calcService.CalculateMonthlyIncome(profile, absences);

            var baseSalary = profile.YearlyIncome / 12;
            var diff = netSalary - baseSalary;

            if (diff < 0)
                SalaryInfo = $"Lön: {netSalary:N0} kr (Avdrag: {diff:N0})";
            else
                SalaryInfo = $"Lön: {netSalary:N0} kr";

            var summary = _calcService.GetBudgetSummary(
                CurrentViewDate.Year,
                CurrentViewDate.Month,
                Transactions.Select(t => t.GetModel()).ToList(),
                netSalary);

            TotalIncome = summary.TotalIncome;
            TotalExpense = summary.TotalExpenses;
            ForecastBalance = summary.ForecastBalance;

            var rawTransactions = _dataService.GetTransactions();

            decimal history = _calcService.GetAccumulatedResult(
                CurrentViewDate.Year,
                CurrentViewDate.Month,
                profile,
                rawTransactions,
                _dataService);

            TotalBalance = history + ForecastBalance;
        }

        private void OnAddTransaction(object? parameter)
        {
            var vm = new AddTransactionViewModel();
            var window = new AddTransactionWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            vm.CloseAction = (success) =>
            {
                window.DialogResult = success;
                window.Close();
            };

            bool? result = window.ShowDialog();

            if (result == true && vm.CreatedTransaction != null)
            {
                _dataService.SaveTransaction(vm.CreatedTransaction);

                Transactions.Add(new TransactionItemViewModel(vm.CreatedTransaction));

                UpdateCalculations();
            }
        }

        private void OnDeleteTransaction(object? parameter)
        {
            if (parameter is TransactionItemViewModel item)
            {
                var result = MessageBox.Show($"Ta bort {item.Name}?", "Bekräfta", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;

                _dataService.DeleteTransaction(item.GetModel().Id);

                Transactions.Remove(item);

                UpdateCalculations();
            }
        }

        private void OnAddAbsence(object? obj)
        {
            var vm = new AddAbsenceViewModel();

            int daysInMonth = DateTime.DaysInMonth(CurrentViewDate.Year, CurrentViewDate.Month);

            int day = Math.Min(DateTime.Now.Day, daysInMonth);

            vm.Date = new DateTime(CurrentViewDate.Year, CurrentViewDate.Month, day);

            var window = new AddAbsenceWindow { DataContext = vm, Owner = Application.Current.MainWindow };

            vm.CloseAction = (success) =>
            {
                window.DialogResult = success;
                window.Close();
            };

            if (window.ShowDialog() == true)
            {
                _dataService.SaveAbsence(vm.CreatedAbsence);
                UpdateCalculations();
            }
        }

        private void SwitchView(string view)
        {
            if (view == "dashboard")
            {
                IsDashboardVisible = true;
                IsSettingsVisible = false;
                LoadData();
            }
            else
            {
                IsDashboardVisible = false;
                IsSettingsVisible = true;
            }
        }
    }
}
