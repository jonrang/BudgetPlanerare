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
using static BudgetPlanerare.Services.CalculationService;

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
                    RaisePropertyChanged(nameof(CurrentMonthHeader));

                    if (IsDashboardVisible)
                    {
                        LoadDashboardData();
                    }
                }
            }
        }

        public SettingsViewModel SettingsVM { get; } = new SettingsViewModel();

        public string CurrentMonthHeader => _currentViewDate.ToString("MMMM yyyy").ToUpper();

        public ObservableCollection<IHistoryItem> Transactions { get; set; } = new();

        private decimal _totalIncome;
        public decimal TotalIncome { get => _totalIncome; set => SetProperty(ref _totalIncome, value); }

        private decimal _totalExpense;
        public decimal TotalExpense { get => _totalExpense; set => SetProperty(ref _totalExpense, value); }

        private decimal _forecastBalance;
        public decimal ForecastBalance { get => _forecastBalance; set => SetProperty(ref _forecastBalance, value); }

        private bool _isDashboardVisible = true;
        public bool IsDashboardVisible { get => _isDashboardVisible; set => SetProperty(ref _isDashboardVisible, value); }

        private bool _isAllTransactionsVisible = false;
        public bool IsAllTransactionsVisible { get => _isAllTransactionsVisible; set => SetProperty(ref _isAllTransactionsVisible, value); }

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
        public DelegateCommand ShowAllTransactionsCommand { get; }
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
            ShowAllTransactionsCommand = new DelegateCommand(_ => SwitchView("all"));
            ShowSettingsCommand = new DelegateCommand(_ => SwitchView("settings"));

            AddAbsenceCommand = new DelegateCommand(OnAddAbsence);

            LoadDashboardData();
        }


        private void UpdateCalculations()
        {
            if (!IsDashboardVisible) return;

            var profile = _dataService.GetUserProfile();

            var absences = _dataService.GetAbsencesForMonth(CurrentViewDate.Year, CurrentViewDate.Month);

            var salaryResult = _calcService.CalculateMonthlyIncome(profile, absences);

            if (salaryResult.Deduction < 0)
                SalaryInfo = $"Lön: {salaryResult.NetSalary:N0} kr (Avdrag: {salaryResult.Deduction:N0})";
            else
                SalaryInfo = $"Lön: {salaryResult.NetSalary:N0} kr";

            var transactionModels = Transactions
                .OfType<TransactionItemViewModel>()
                .Select(t => t.GetModel())
                .ToList();

            var summary = _calcService.GetBudgetSummary(
                CurrentViewDate.Year,
                CurrentViewDate.Month,
                transactionModels,
                salaryResult.NetSalary);

            TotalIncome = summary.TotalIncome;
            TotalExpense = summary.TotalExpenses;
            ForecastBalance = summary.ForecastBalance;

            var allTransactions = _dataService.GetTransactions();
            var allAbsences = _dataService.GetAllAbsences();

            decimal history = _calcService.GetAccumulatedResult(
                CurrentViewDate.Year,
                CurrentViewDate.Month,
                profile,
                allTransactions,
                allAbsences);

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
            if (parameter is IHistoryItem item)
            {
                var msg = item.IsTransaction ?
                    $"Ta bort transaktion: {item.Name}?" :
                    $"Ta bort frånvaro: {item.DisplayDate}?";

                if (MessageBox.Show(msg, "Ta bort", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (item.IsTransaction)
                    {
                        _dataService.DeleteTransaction(item.Id);
                    }
                    else
                    {
                        _dataService.DeleteAbsence(item.Id);
                    }

                    Transactions.Remove(item);
                    UpdateCalculations();
                }
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
            IsDashboardVisible = (view == "dashboard");
            IsAllTransactionsVisible = (view == "all");
            IsSettingsVisible = (view == "settings");

            if (IsDashboardVisible)
            {
                LoadDashboardData();
            }
            else if (IsAllTransactionsVisible)
            {
                LoadHistoryData();
            }
        }

        private void LoadDashboardData()
        {
            Transactions.Clear();
            var allTrans = _dataService.GetTransactions(); 

            foreach (var t in allTrans)
            {
                bool include = false;

                if (t.Frequency == Frequency.Monthly) include = true;
                else if (t.Frequency == Frequency.Yearly && t.YearlyOccurringMonth == CurrentViewDate.Month) include = true;
                else if (t.Frequency == Frequency.OneTime && t.Date.Year == CurrentViewDate.Year && t.Date.Month == CurrentViewDate.Month) include = true;

                if (include)
                {
                    Transactions.Add(new TransactionItemViewModel(t));
                }
            }

            UpdateCalculations();
        }

        private void LoadHistoryData()
        {
            Transactions.Clear();

            var allTrans = _dataService.GetTransactions();
            var historyList = new List<IHistoryItem>();

            foreach (var t in allTrans)
            {
                historyList.Add(new TransactionItemViewModel(t));
            }

            var allAbsence = _dataService.GetAllAbsences(); 
            var profile = _dataService.GetUserProfile();

            decimal hourlyRate = (profile.YearlyWorkHours > 0) ? profile.YearlyIncome / profile.YearlyWorkHours : 0;
            decimal dayCost = hourlyRate * 8;

            foreach (var a in allAbsence)
            {
                historyList.Add(new AbsenceItemViewModel(a, dayCost));
            }

            var sortedList = historyList.OrderByDescending(x => x.Date).ToList();

            foreach (var item in sortedList)
            {
                Transactions.Add(item);
            }
        }
    }
}
