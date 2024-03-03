using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_ASP_NET.Migrations
{
    public partial class ConstraintMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Categories_CategoryId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Categories_CategoryId",
                table: "Groups",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Categories_CategoryId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Groups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Categories_CategoryId",
                table: "Groups",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
