using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class SetOnDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads");

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads");

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
