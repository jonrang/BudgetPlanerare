using System.Configuration;
using System.Data;
using System.Windows;
using BudgetPlanerare.Services;

namespace BudgetPlanerare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                // Alternativ B: Använd detta om du kör Migrationer (Rekommenderas för "riktiga" projekt)
                // db.Database.Migrate(); 
            }
        }
    }

}
