using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.VM;

namespace BudgetPlanerare.ViewModels
{
    public class CategoryItemViewModel : ViewModelBase
    {
        private readonly Category _model;

        public CategoryItemViewModel(Category model)
        {
            _model = model;
        }

        public int Id => _model.Id;

        public string Name => _model.Name;

        
        public string GroupName => _model.IsIncome ? "Inkomster" : "Utgifter";

        public Category Model => _model;
    }
}
