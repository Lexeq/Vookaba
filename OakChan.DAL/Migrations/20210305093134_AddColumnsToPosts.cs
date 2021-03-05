using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class AddColumnsToPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorIP",
                table: "Posts",
                nullable: false,
                defaultValue: "n/d");

            migrationBuilder.AddColumn<string>(
                name: "AuthorUserAgent",
                table: "Posts",
                nullable: false,
                defaultValue: "n/d");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorIP",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AuthorUserAgent",
                table: "Posts");
        }
    }
}
