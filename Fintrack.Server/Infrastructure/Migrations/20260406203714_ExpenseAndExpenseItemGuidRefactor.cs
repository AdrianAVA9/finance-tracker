using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations;

/// <inheritdoc />
public partial class ExpenseAndExpenseItemGuidRefactor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ExpenseItems_Expenses_ExpenseId",
            table: "ExpenseItems");

        migrationBuilder.Sql(
            """
            ALTER TABLE "Expenses" ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
            ALTER TABLE "Expenses"
                ALTER COLUMN "Id" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

            ALTER TABLE "ExpenseItems"
                ALTER COLUMN "ExpenseId" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("ExpenseId"::text, 12, '0'))::uuid);

            ALTER TABLE "ExpenseItems" ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
            ALTER TABLE "ExpenseItems"
                ALTER COLUMN "Id" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);
            """);

        migrationBuilder.AddForeignKey(
            name: "FK_ExpenseItems_Expenses_ExpenseId",
            table: "ExpenseItems",
            column: "ExpenseId",
            principalTable: "Expenses",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException(
            "Down migration is not supported for ExpenseAndExpenseItemGuidRefactor.");
    }
}
