using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseCategoryGuidRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_ExpenseCategories_CategoryId",
                schema: "app",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseItems_ExpenseCategories_CategoryId",
                table: "ExpenseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_ExpenseCategories_AssignedCategoryId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringExpenses_ExpenseCategories_CategoryId",
                table: "RecurringExpenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseCategories",
                table: "ExpenseCategories");

            migrationBuilder.Sql("""
                ALTER TABLE "ExpenseCategories" ALTER COLUMN "Id" DROP DEFAULT;
                ALTER TABLE "ExpenseCategories"
                    ALTER COLUMN "Id" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

                ALTER TABLE "ExpenseItems"
                    ALTER COLUMN "CategoryId" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("CategoryId"::text, 12, '0'))::uuid);

                ALTER TABLE "RecurringExpenses"
                    ALTER COLUMN "CategoryId" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("CategoryId"::text, 12, '0'))::uuid);

                ALTER TABLE "InvoiceItems"
                    ALTER COLUMN "AssignedCategoryId" TYPE uuid
                    USING (CASE
                        WHEN "AssignedCategoryId" IS NULL THEN NULL
                        ELSE ('00000000-0000-0000-0000-' || lpad("AssignedCategoryId"::text, 12, '0'))::uuid
                    END);

                ALTER TABLE "Expenses"
                    ALTER COLUMN "ExpenseCategoryId" TYPE uuid
                    USING (CASE
                        WHEN "ExpenseCategoryId" IS NULL THEN NULL
                        ELSE ('00000000-0000-0000-0000-' || lpad("ExpenseCategoryId"::text, 12, '0'))::uuid
                    END);

                ALTER TABLE app."Budgets"
                    ALTER COLUMN "CategoryId" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("CategoryId"::text, 12, '0'))::uuid);
                """);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseCategories",
                table: "ExpenseCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_ExpenseCategories_CategoryId",
                schema: "app",
                table: "Budgets",
                column: "CategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseItems_ExpenseCategories_CategoryId",
                table: "ExpenseItems",
                column: "CategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_ExpenseCategories_AssignedCategoryId",
                table: "InvoiceItems",
                column: "AssignedCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringExpenses_ExpenseCategories_CategoryId",
                table: "RecurringExpenses",
                column: "CategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException("Down migration is not supported for ExpenseCategoryGuidRefactor.");
        }
    }
}
