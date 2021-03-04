using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class UpdateTokensTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IP",
                table: "IdTokens");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "IdTokens",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "IdTokens",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdTokens_UserId",
                table: "IdTokens",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdTokens_AspNetUsers_UserId",
                table: "IdTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdTokens_AspNetUsers_UserId",
                table: "IdTokens");

            migrationBuilder.DropIndex(
                name: "IX_IdTokens_UserId",
                table: "IdTokens");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdTokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "IdTokens",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "IdTokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
