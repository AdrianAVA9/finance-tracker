using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations;

/// <inheritdoc />
public partial class IncomePrimaryKeyGuid : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // PostgreSQL cannot cast integer PK to uuid in place. Add new column, swap PK (preserves rows).
        migrationBuilder.Sql(
            """
            ALTER TABLE "Incomes" ADD COLUMN "Id_temp" uuid NOT NULL DEFAULT gen_random_uuid();
            ALTER TABLE "Incomes" DROP CONSTRAINT "PK_Incomes";
            ALTER TABLE "Incomes" DROP COLUMN "Id";
            ALTER TABLE "Incomes" RENAME COLUMN "Id_temp" TO "Id";
            ALTER TABLE "Incomes" ADD CONSTRAINT "PK_Incomes" PRIMARY KEY ("Id");
            ALTER TABLE "Incomes" ALTER COLUMN "Id" DROP DEFAULT;
            """);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Down is not supported: original integer identifiers cannot be reconstructed from Guid values.
    /// Restore from backup if rollback is required.
    /// </remarks>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException(
            "Down migration for IncomePrimaryKeyGuid is not supported. Restore from backup if rollback is required.");
    }
}
