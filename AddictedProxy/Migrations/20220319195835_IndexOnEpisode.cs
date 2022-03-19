using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class IndexOnEpisode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId_Season_Number",
                table: "Episodes",
                columns: new[] { "TvShowId", "Season", "Number" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Episodes_TvShowId_Season_Number",
                table: "Episodes");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes",
                column: "TvShowId");
        }
    }
}
