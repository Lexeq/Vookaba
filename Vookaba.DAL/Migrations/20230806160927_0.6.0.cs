using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vookaba.DAL.Migrations
{
    public partial class _060 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BannedNetwork = table.Column<ValueTuple<IPAddress, int>>(type: "cidr", nullable: true),
                    Expired = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BannedAothorToken = table.Column<Guid>(type: "uuid", nullable: true),
                    BoardKey = table.Column<string>(type: "character varying(10)", nullable: true),
                    IsCanceled = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bans_AuthorTokens_BannedAothorToken",
                        column: x => x.BannedAothorToken,
                        principalTable: "AuthorTokens",
                        principalColumn: "Token");
                    table.ForeignKey(
                        name: "FK_Bans_Boards_BoardKey",
                        column: x => x.BoardKey,
                        principalTable: "Boards",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_Bans_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bans_BannedAothorToken",
                table: "Bans",
                column: "BannedAothorToken");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_BoardKey",
                table: "Bans",
                column: "BoardKey");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_IsCanceled_Expired_BoardKey_BannedNetwork_BannedAothor~",
                table: "Bans",
                columns: new[] { "IsCanceled", "Expired", "BoardKey", "BannedNetwork", "BannedAothorToken" });

            migrationBuilder.CreateIndex(
                name: "IX_Bans_PostId",
                table: "Bans",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bans");
        }
    }
}
