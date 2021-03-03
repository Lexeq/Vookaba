using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class Renames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Anonymous",
                newName: "IdTokens");

            migrationBuilder.RenameColumn(
                table: "Posts",
                name: "UserId",
                newName: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "IdTokens",
                newName: "Anonymous");

            migrationBuilder.RenameColumn(
                table: "Posts",
                name: "AuthorId",
                newName: "UserId");
        }
    }
}
