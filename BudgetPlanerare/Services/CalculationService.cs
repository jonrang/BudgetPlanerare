using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.Service;

namespace BudgetPlanerare.Services
{
    public class CalculationService
    {
        private const decimal VAB_CAP = 410000m;

        public class SalaryResult
        {
            public decimal NetSalary { get; set; }   
            public decimal BaseSalary { get; set; }    // Grundlön (årslön / 12)
            public decimal Deduction => NetSalary - BaseSalary; 
        }

        public SalaryResult CalculateMonthlyIncome(UserProfile profile, List<Absence> absences)
        {
            decimal baseMonthlySalary = profile.YearlyIncome / 12;
            decimal hourlyRate = profile.YearlyIncome / profile.YearlyWorkHours;

            decimal vabHourlyRate = Math.Min(profile.YearlyIncome, VAB_CAP) / profile.YearlyWorkHours;

            decimal totalDeduction = 0;
            decimal totalCompensation = 0;

            foreach (var absence in absences)
            {
                decimal currentRate = (absence.Type == AbsenceType.VAB) ? vabHourlyRate : hourlyRate;
                decimal dayDeduction = currentRate * 8;

                totalDeduction += dayDeduction;
                totalCompensation += dayDeduction * 0.8m;
            }

            return new SalaryResult
            {
                BaseSalary = baseMonthlySalary,
                NetSalary = baseMonthlySalary - totalDeduction + totalCompensation
            };
        }

        //public decimal GetForecastForMonth(int year, int month, List<Transaction> transactions, decimal netSalary)
        //{
        //    decimal totalIncome = netSalary;
        //    decimal totalExpenses = 0;

        //    foreach (var t in transactions)
        //    {
        //        bool include = false;

        //        if (t.Frequency == Frequency.Monthly) include = true;
        //        else if (t.Frequency == Frequency.Yearly && t.YearlyOccurringMonth == month) include = true;
        //        else if (t.Frequency == Frequency.OneTime && t.Date.Month == month && t.Date.Year == year) include = true;

        //        if (include)
        //        {
        //            if (t.Category != null && t.Category.IsIncome) totalIncome += t.Amount;
        //            else totalExpenses += t.Amount;
        //        }
        //    }

        //    return totalIncome - totalExpenses;
        //}

        public MonthlyBudgetSummary GetBudgetSummary(int year, int month, List<Transaction> transactions, decimal netSalary)
        {
            decimal income = netSalary; 
            decimal expenses = 0;

            foreach (var t in transactions)
            {
                bool include = false;

                if (t.Frequency == Frequency.Monthly) include = true;
                else if (t.Frequency == Frequency.Yearly && t.YearlyOccurringMonth == month) include = true;
                else if (t.Frequency == Frequency.OneTime && t.Date.Month == month && t.Date.Year == year) include = true;

                if (include)
                {
                    if (t.Category != null && t.Category.IsIncome)
                        income += t.Amount;
                    else
                        expenses += t.Amount;
                }
            }

            return new MonthlyBudgetSummary
            {
                TotalIncome = income,
                TotalExpenses = expenses,
                ForecastBalance = income - expenses
            };
        }

        public decimal GetAccumulatedResult(int currentYear, int currentMonth, UserProfile profile, List<Transaction> allTransactions, List<Absence> allAbsences)
        {
            decimal accumulated = profile.StartingBalance;

            DateTime iterator = profile.AppStartDate;

            iterator = new DateTime(iterator.Year, iterator.Month, 1);

            DateTime target = new DateTime(currentYear, currentMonth, 1);

            while (iterator < target)
            {
                var absencesThisMonth = allAbsences
                .Where(a => a.Date.Year == iterator.Year && a.Date.Month == iterator.Month)
                .ToList();

                var salaryResults = CalculateMonthlyIncome(profile, absencesThisMonth);

                var summary = GetBudgetSummary(iterator.Year, iterator.Month, allTransactions, salaryResults.NetSalary);

                accumulated += summary.ForecastBalance;

                iterator = iterator.AddMonths(1);
            }

            return accumulated;
        }
    }
}
