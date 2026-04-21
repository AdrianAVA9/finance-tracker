using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations;

/// <inheritdoc />
public partial class InvoiceAndInvoiceItemGuidRefactor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_InvoiceItems_Invoices_InvoiceId",
            schema: "app",
            table: "InvoiceItems");

        migrationBuilder.DropForeignKey(
            name: "FK_Expenses_Invoices_InvoiceId",
            schema: "app",
            table: "Expenses");

        migrationBuilder.Sql(
            """
            ALTER TABLE app."Invoices" ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
            ALTER TABLE app."Invoices"
                ALTER COLUMN "Id" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

            ALTER TABLE app."InvoiceItems"
                ALTER COLUMN "InvoiceId" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("InvoiceId"::text, 12, '0'))::uuid);

            ALTER TABLE app."InvoiceItems" ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
            ALTER TABLE app."InvoiceItems"
                ALTER COLUMN "Id" TYPE uuid
                USING (('00000000-0000-0000-0000-' || lpad("Id"::text, 12, '0'))::uuid);

            ALTER TABLE app."Expenses"
                ALTER COLUMN "InvoiceId" TYPE uuid
                USING (
                    CASE WHEN "InvoiceId" IS NULL THEN NULL
                    ELSE ('00000000-0000-0000-0000-' || lpad("InvoiceId"::text, 12, '0'))::uuid
                    END);
            """);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            schema: "app",
            table: "Invoices",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero));

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "app",
            table: "Invoices",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastModifiedBy",
            schema: "app",
            table: "Invoices",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            schema: "app",
            table: "Invoices",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            schema: "app",
            table: "InvoiceItems",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero));

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "app",
            table: "InvoiceItems",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastModifiedBy",
            schema: "app",
            table: "InvoiceItems",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            schema: "app",
            table: "InvoiceItems",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_InvoiceItems_Invoices_InvoiceId",
            schema: "app",
            table: "InvoiceItems",
            column: "InvoiceId",
            principalSchema: "app",
            principalTable: "Invoices",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Expenses_Invoices_InvoiceId",
            schema: "app",
            table: "Expenses",
            column: "InvoiceId",
            principalSchema: "app",
            principalTable: "Invoices",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Down is not supported: original integer identifiers cannot be reconstructed from Guid values.
    /// </remarks>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException(
            "Down migration for InvoiceAndInvoiceItemGuidRefactor is not supported. Restore from backup if rollback is required.");
    }
}
