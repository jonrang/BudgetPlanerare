using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetPlanerare.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public decimal YearlyIncome { get; set; }
        public int YearlyWorkHours { get; set; }
    }
}
