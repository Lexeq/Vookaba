using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class AddIsHiddenPropertyToBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Boards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Boards");
        }
    }
}
