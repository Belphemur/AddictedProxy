using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAtUpdatedAtColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TvShows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TvShows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Subtitles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Subtitles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Seasons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Seasons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Episodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Episodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_TvdbId",
                table: "TvShows",
                column: "TvdbId");
            
            migrationBuilder.Sql($@"UPDATE ""TvShows"" SET ""CreatedAt"" = ""Discovered"", ""UpdatedAt"" = NOW() at time zone 'utc'");
            migrationBuilder.Sql($@"UPDATE ""Subtitles"" SET ""CreatedAt"" = ""Discovered"", ""UpdatedAt"" = NOW() at time zone 'utc'");
            migrationBuilder.Sql($@"UPDATE ""Episodes"" SET ""CreatedAt"" = ""Discovered"", ""UpdatedAt"" = NOW() at time zone 'utc'");
            migrationBuilder.Sql($@"UPDATE ""Seasons"" SET ""CreatedAt"" = NOW() at time zone 'utc', ""UpdatedAt"" = NOW() at time zone 'utc'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TvShows_TvdbId",
                table: "TvShows");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TvShows");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TvShows");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Episodes");
        }
    }
}
