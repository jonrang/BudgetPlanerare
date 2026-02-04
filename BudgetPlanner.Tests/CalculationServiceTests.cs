using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.Services;
using Xunit;

namespace BudgetPlanner.Tests
{
    public class CalculationServiceTests
    {
        [Fact]
        public void CalulateMonthlyIncome_VabCapLimit()
        {
            // Arrange
            var service = new CalculationService();
            var profile = new UserProfile
            {
                YearlyIncome = 600000m, // Högre än taket på 410 000
                YearlyWorkHours = 2000
            };
            var absences = new List<Absence>
        {
            new Absence { Date = DateTime.Now, Type = AbsenceType.VAB } // 1 dag VAB
        };

            // Manuella beräkningar för kontroll:
            // Bas-månadslön: 600 000 / 12 = 50 000
            // VAB-timlön (tak): 410 000 / 2000 = 205 kr/h
            // Avdrag (8h): 205 * 8 = 1640 kr
            // Ersättning (80%): 1640 * 0.8 = 1312 kr
            // Förväntat netto: 50 000 - 1640 + 1312 = 49 672 kr
            decimal expectedNet = 49672m;

            // Act
            var result = service.CalculateMonthlyIncome(profile, absences);

            // Assert
            Assert.Equal(expectedNet, result.NetSalary);
        }

        [Fact]
        public void GetForecastForMonth_ShouldIncludeYearlyExpense_OnlyInCorrectMonth()
        {
            // Arrange
            var service = new CalculationService();
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Name = "Bilförsäkring",
                    Amount = 5000m,
                    Frequency = Frequency.Yearly,
                    YearlyOccurringMonth = 5, // Maj
                    Category = new Category { IsIncome = false }
                },
                new Transaction
                {
                    Name = "Hyra",
                    Amount = 10000m,
                    Frequency = Frequency.Monthly,
                    Category = new Category { IsIncome = false }
                }
            };
            decimal netSalary = 25000m;

            // Act
            var summaryMay = service.GetBudgetSummary(2024, 5, transactions, netSalary);
            var summaryJune = service.GetBudgetSummary(2024, 6, transactions, netSalary);

            // Assert
            // Maj: 25000 - 10000 - 5000 = 10000
            Assert.Equal(10000m, summaryMay.ForecastBalance);

            // Juni: 25000 - 10000 = 15000
            Assert.Equal(15000m, summaryJune.ForecastBalance);
        }
    }
}
