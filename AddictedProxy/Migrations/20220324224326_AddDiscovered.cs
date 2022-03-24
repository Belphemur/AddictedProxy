using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class AddDiscovered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Discovered",
                table: "TvShows",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_DownloadUri",
                table: "Subtitles",
                column: "DownloadUri",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subtitles_DownloadUri",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "Discovered",
                table: "TvShows");
        }
    }
}
