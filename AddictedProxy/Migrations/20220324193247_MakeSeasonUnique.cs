using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class MakeSeasonUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_TvShowId",
                table: "Seasons");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_TvShowId_Number",
                table: "Seasons",
                columns: new[] { "TvShowId", "Number" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_TvShowId_Number",
                table: "Seasons");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_TvShowId",
                table: "Seasons",
                column: "TvShowId");
        }
    }
}
