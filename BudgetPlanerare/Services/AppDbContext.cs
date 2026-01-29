using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanerare.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<TransactionModifier> TransactionModifiers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=budget.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Lön", IsIncome = true },
                new Category { Id = 2, Name = "Bidrag", IsIncome = true },
                new Category { Id = 3, Name = "Mat", IsIncome = false },
                new Category { Id = 4, Name = "Hus & drift", IsIncome = false },
                new Category { Id = 5, Name = "Transport", IsIncome = false },
                new Category { Id = 6, Name = "Streaming-tjänster", IsIncome = false },
                new Category { Id = 7, Name = "Barn", IsIncome = false },
                new Category { Id = 8, Name = "Fritid", IsIncome = false }
                );
        }
    }
}
