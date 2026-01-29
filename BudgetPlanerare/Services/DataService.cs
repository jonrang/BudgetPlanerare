using System;
using System.Collections.Generic;
using System.Text;
using BudgetPlanerare.Models;
using BudgetPlanerare.Services;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanerare.Service
{
    public class DataService
    {
        public List<Transaction> GetTransactions()
        {
            using var db = new AppDbContext();
            return db.Transactions.Include(t => t.Category).ToList();
        }

        public void SaveTransaction(Transaction transaction)
        {
            using var db = new AppDbContext();
            if (transaction.Category != null)
            {
                db.Attach(transaction.Category);
                db.Entry(transaction.Category).State = EntityState.Unchanged;
            }
            if (transaction.Id == 0) db.Transactions.Add(transaction);
            else db.Transactions.Update(transaction);
            db.SaveChanges();
        }

        public void DeleteTransaction(int id)
        {
            using var db = new AppDbContext();
            var t = db.Transactions.Find(id);
            if (t != null)
            {
                db.Transactions.Remove(t);
                db.SaveChanges();
            }
        }

        public UserProfile GetUserProfile()
        {
            using var db = new AppDbContext();
            return db.UserProfiles.FirstOrDefault() ?? new UserProfile { YearlyIncome = 0, YearlyWorkHours = 2080 };
        }

        public void SaveUserProfile(UserProfile profile)
        {
            using var db = new AppDbContext();

            var existing = db.UserProfiles.FirstOrDefault(p => p.Id == profile.Id);

            if (existing == null)
            {
                db.UserProfiles.Add(profile);
            }
            else
            {
                existing.YearlyIncome = profile.YearlyIncome;
                existing.YearlyWorkHours = profile.YearlyWorkHours;
            }

            db.SaveChanges();
        }

        public List<Absence> GetAbsencesForMonth(int year, int month)
        {
            using var db = new AppDbContext();
            return db.Absences
                .Where(a => a.Date.Year == year && a.Date.Month == month)
                .ToList();
        }

        public void SaveAbsence(Absence absence)
        {
            using var db = new AppDbContext();
            if (absence.Id == 0) db.Absences.Add(absence);
            else db.Absences.Update(absence);
            db.SaveChanges();
        }

        public void DeleteAbsence(int id)
        {
            using var db = new AppDbContext();
            var item = db.Absences.Find(id);
            if (item != null)
            {
                db.Absences.Remove(item);
                db.SaveChanges();
            }
        }

        public bool HasAbsenceOnDate(DateTime date)
        {
            using var db = new AppDbContext();
            return db.Absences.Any(a => a.Date.Date == date.Date);
        }
    }
}
