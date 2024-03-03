using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_ASP_NET.Migrations
{
    public partial class PictureRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
