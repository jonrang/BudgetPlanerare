using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BudgetPlanerare.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsRepeating { get; set; }
        public DateTime? EndDate { get; set; }
        public Frequency Frequency { get; set; }
        public int? YearlyOccurringMonth { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual ICollection<TransactionModifier> Modifiers { get; set; } = new List<TransactionModifier>();
    }
}
