using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseCategoryGroupGuidRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_ExpenseCategoryGroups_GroupId",
                table: "ExpenseCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseCategoryGroups",
                table: "ExpenseCategoryGroups");

            migrationBuilder.Sql("""
                ALTER TABLE "ExpenseCategoryGroups" ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
                ALTER TABLE "ExpenseCategoryGroups"
                    ALTER COLUMN "Id" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

                ALTER TABLE "ExpenseCategories"
                    ALTER COLUMN "GroupId" TYPE uuid
                    USING (CASE
                        WHEN "GroupId" IS NULL THEN NULL
                        ELSE ('00000000-0000-0000-0000-' || lpad("GroupId"::text, 12, '0'))::uuid
                    END);
                """);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseCategoryGroups",
                table: "ExpenseCategoryGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_ExpenseCategoryGroups_GroupId",
                table: "ExpenseCategories",
                column: "GroupId",
                principalTable: "ExpenseCategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException("Down migration is not supported for ExpenseCategoryGroupGuidRefactor.");
        }
    }
}
