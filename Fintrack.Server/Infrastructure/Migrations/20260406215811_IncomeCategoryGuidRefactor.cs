using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class IncomeCategoryGuidRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_IncomeCategories_CategoryId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringIncomes_IncomeCategories_CategoryId",
                table: "RecurringIncomes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomeCategories",
                table: "IncomeCategories");

            migrationBuilder.AddColumn<bool>(
                name: "IsEditable",
                table: "IncomeCategories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(
                """UPDATE "IncomeCategories" SET "IsEditable" = false WHERE "UserId" IS NULL;""");

            migrationBuilder.Sql(
                """
                ALTER TABLE "IncomeCategories" ALTER COLUMN "Id" DROP DEFAULT;
                ALTER TABLE "IncomeCategories"
                    ALTER COLUMN "Id" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

                ALTER TABLE "Incomes"
                    ALTER COLUMN "CategoryId" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("CategoryId"::text, 12, '0'))::uuid);

                ALTER TABLE "RecurringIncomes"
                    ALTER COLUMN "CategoryId" TYPE uuid
                    USING (('00000000-0000-0000-0000-' || lpad("CategoryId"::text, 12, '0'))::uuid);
                """);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomeCategories",
                table: "IncomeCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_IncomeCategories_CategoryId",
                table: "Incomes",
                column: "CategoryId",
                principalTable: "IncomeCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringIncomes_IncomeCategories_CategoryId",
                table: "RecurringIncomes",
                column: "CategoryId",
                principalTable: "IncomeCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException("Down migration is not supported for IncomeCategoryGuidRefactor.");
        }
    }
}
