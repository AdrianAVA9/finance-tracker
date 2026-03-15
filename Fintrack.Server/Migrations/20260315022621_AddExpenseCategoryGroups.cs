using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseCategoryGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ExpenseCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExpenseCategories",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ExpenseCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ExpenseCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpenseCategoryGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsEditable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategoryGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseCategoryGroups_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_GroupId",
                table: "ExpenseCategories",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategoryGroups_UserId",
                table: "ExpenseCategoryGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_ExpenseCategoryGroups_GroupId",
                table: "ExpenseCategories",
                column: "GroupId",
                principalTable: "ExpenseCategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_ExpenseCategoryGroups_GroupId",
                table: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "ExpenseCategoryGroups");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_GroupId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ExpenseCategories");
        }
    }
}
