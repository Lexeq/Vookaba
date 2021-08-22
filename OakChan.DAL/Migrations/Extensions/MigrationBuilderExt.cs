using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.Reflection;

namespace OakChan.DAL.Migrations
{
    static class MigrationBuilderExt
    {
        public static void SqlFromResource(this MigrationBuilder migrationBuilder, string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            migrationBuilder.Sql(reader.ReadToEnd());
        }
    }
}
