using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fintrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpenseCategorySystemToEditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ExpenseCategories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsEditable",
                table: "ExpenseCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "IsEditable",
                table: "ExpenseCategories");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ExpenseCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
