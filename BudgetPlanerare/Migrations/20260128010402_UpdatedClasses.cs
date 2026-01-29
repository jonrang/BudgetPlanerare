using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetPlanerare.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearlyMonthMonth",
                table: "Transactions",
                newName: "YearlyOccurringMonth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearlyOccurringMonth",
                table: "Transactions",
                newName: "YearlyMonthMonth");
        }
    }
}
