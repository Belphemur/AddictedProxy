using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class RemoveUniqueConstraintSubtitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Subtitles_EpisodeId_Language_Version");

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_EpisodeId_Language_Version",
                table: "Subtitles",
                columns: new[] { "EpisodeId", "Language", "Version" },
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Subtitles_EpisodeId_Language_Version");

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_EpisodeId_Language_Version",
                table: "Subtitles",
                columns: new[] { "EpisodeId", "Language", "Version" },
                unique: true);
        }
    }
}
