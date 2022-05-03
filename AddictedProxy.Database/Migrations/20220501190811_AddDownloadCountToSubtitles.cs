using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddDownloadCountToSubtitles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DownloadCount",
                table: "Subtitles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "Subtitles");
        }
    }
}
