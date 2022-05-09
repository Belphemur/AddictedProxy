using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddCompletionPctSubtitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CompletionPct",
                table: "Subtitles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            var sql = "UPDATE Subtitles SET CompletionPct = 100.0 WHERE Completed = true";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionPct",
                table: "Subtitles");
        }
    }
}
