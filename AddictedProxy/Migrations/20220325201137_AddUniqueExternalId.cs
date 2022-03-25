using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class AddUniqueExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows",
                column: "ExternalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows");
        }
    }
}
