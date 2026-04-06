using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToAppAndIdentitySchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.RenameTable(
                name: "SavingsGoals",
                newName: "SavingsGoals",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "RecurringIncomes",
                newName: "RecurringIncomes",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "RecurringExpenses",
                newName: "RecurringExpenses",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoices",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "InvoiceItems",
                newName: "InvoiceItems",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Incomes",
                newName: "Incomes",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "IncomeCategories",
                newName: "IncomeCategories",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Expenses",
                newName: "Expenses",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ExpenseItems",
                newName: "ExpenseItems",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ExpenseCategoryGroups",
                newName: "ExpenseCategoryGroups",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ExpenseCategories",
                newName: "ExpenseCategories",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "identity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SavingsGoals",
                schema: "app",
                newName: "SavingsGoals",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RecurringIncomes",
                schema: "app",
                newName: "RecurringIncomes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RecurringExpenses",
                schema: "app",
                newName: "RecurringExpenses",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Invoices",
                schema: "app",
                newName: "Invoices",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "InvoiceItems",
                schema: "app",
                newName: "InvoiceItems",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Incomes",
                schema: "app",
                newName: "Incomes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "IncomeCategories",
                schema: "app",
                newName: "IncomeCategories",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Expenses",
                schema: "app",
                newName: "Expenses",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExpenseItems",
                schema: "app",
                newName: "ExpenseItems",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExpenseCategoryGroups",
                schema: "app",
                newName: "ExpenseCategoryGroups",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExpenseCategories",
                schema: "app",
                newName: "ExpenseCategories",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "identity",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "identity",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "identity",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "identity",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "identity",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "identity",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "identity",
                newName: "AspNetRoleClaims",
                newSchema: "public");
        }
    }
}
