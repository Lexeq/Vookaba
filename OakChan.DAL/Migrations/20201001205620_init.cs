using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OakChan.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anonymous",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IP = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anonymous", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalName = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Size = table.Column<int>(nullable: false),
                    Hash = table.Column<byte[]>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BoardId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Threads_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false, defaultValue: "Аноним"),
                    ThreadId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(maxLength: 4096, nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ImageId = table.Column<int>(nullable: true),
                    Subject = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Anonymous_UserId",
                        column: x => x.UserId,
                        principalTable: "Anonymous",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ImageId",
                table: "Posts",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ThreadId",
                table: "Posts",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_BoardId",
                table: "Threads",
                column: "BoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Threads");

            migrationBuilder.DropTable(
                name: "Anonymous");

            migrationBuilder.DropTable(
                name: "Boards");
        }
    }
}
