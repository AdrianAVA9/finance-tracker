using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRecurrentToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRecurrent",
                table: "Budgets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRecurrent",
                table: "Budgets");
        }
    }
}
