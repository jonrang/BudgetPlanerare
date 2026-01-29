using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetPlanerare.Models
{
    public class Absence
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public AbsenceType Type { get; set; }
    }
}
