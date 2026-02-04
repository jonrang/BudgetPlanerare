using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetPlanerare.Models
{
    public interface IHistoryItem
    {
        int Id { get; }
        DateTime Date { get; }
        string DisplayDate { get; }
        string Name { get; }      
        string CategoryName { get; } 
        string TypeName { get; }   
        string FormattedAmount { get; }
        string DisplayColor { get; }
        bool IsTransaction { get; }  
    }
}
