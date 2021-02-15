using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class AddIsDisableddPropertyToBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "Boards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "Boards");
        }
    }
}
