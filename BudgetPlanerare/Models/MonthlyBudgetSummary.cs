using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetPlanerare.Models
{
    public class MonthlyBudgetSummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ForecastBalance { get; set; }
    }
}
