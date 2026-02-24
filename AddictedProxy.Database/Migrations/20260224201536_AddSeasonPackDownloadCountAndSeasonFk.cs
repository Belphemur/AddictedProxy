using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonPackDownloadCountAndSeasonFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DownloadCount",
                table: "SeasonPackSubtitles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SeasonId",
                table: "SeasonPackSubtitles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackSubtitles_SeasonId",
                table: "SeasonPackSubtitles",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonPackSubtitles_Seasons_SeasonId",
                table: "SeasonPackSubtitles",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonPackSubtitles_Seasons_SeasonId",
                table: "SeasonPackSubtitles");

            migrationBuilder.DropIndex(
                name: "IX_SeasonPackSubtitles_SeasonId",
                table: "SeasonPackSubtitles");

            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "SeasonPackSubtitles");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "SeasonPackSubtitles");
        }
    }
}
