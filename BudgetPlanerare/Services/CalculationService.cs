using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;

namespace BudgetPlanerare.Services
{
    public class CalculationService
    {
        private const decimal VAB_CAP = 410000m;

        public decimal CalculateMonthlyIncome(UserProfile profile, List<Absence> absences)
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

            return baseMonthlySalary - totalDeduction + totalCompensation;
        }

        public decimal GetForecastForMonth(int year, int month, List<Transaction> transactions, decimal netSalary)
        {
            decimal totalIncome = netSalary;
            decimal totalExpenses = 0;

            foreach (var t in transactions)
            {
                bool include = false;

                if (t.Frequency == Frequency.Monthly) include = true;
                else if (t.Frequency == Frequency.Yearly && t.YearlyOccurringMonth == month) include = true;
                else if (t.Frequency == Frequency.OneTime && t.Date.Month == month && t.Date.Year == year) include = true;

                if (include)
                {
                    if (t.Category != null && t.Category.IsIncome) totalIncome += t.Amount;
                    else totalExpenses += t.Amount;
                }
            }

            return totalIncome - totalExpenses;
        }

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
    }
}
