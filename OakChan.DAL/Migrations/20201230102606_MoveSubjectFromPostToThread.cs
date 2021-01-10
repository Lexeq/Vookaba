using Microsoft.EntityFrameworkCore.Migrations;

namespace OakChan.Migrations
{
    public partial class MoveSubjectFromPostToThread : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Threads",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE public.\"Threads\" " +
                "SET \"Subject\" = (SELECT p.\"Subject\" FROM public.\"Posts\" p WHERE public.\"Threads\".\"Id\" = p.\"ThreadId\" ORDER BY p.\"CreationTime\" LIMIT 1)");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Threads");

            migrationBuilder.Sql(
                "UPDATE public.\"Posts\" " +
                "SET \"Subject\" = (SELECT t.\"Subject\" FROM public.\"Threads\" t WHERE t.\"Id\" = public.\"Posts\".\"ThreadId\") " +
                "WHERE public.\"Posts\".\"Id\" in  (SELECT \"Id\" FROM public.\"Posts\" GROUP BY \"ThreadId\"");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Posts",
                type: "text",
                nullable: true);
        }
    }
}
