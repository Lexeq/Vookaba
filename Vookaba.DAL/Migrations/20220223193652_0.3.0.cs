using Microsoft.EntityFrameworkCore.Migrations;

namespace Vookaba.DAL.Migrations
{
    public partial class _030 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OpMark",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tripcode",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpMark",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tripcode",
                table: "Posts");
        }
    }
}
