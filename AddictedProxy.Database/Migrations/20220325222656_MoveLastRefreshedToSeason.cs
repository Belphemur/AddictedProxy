using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class MoveLastRefreshedToSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEpisodeRefreshed",
                table: "TvShows");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRefreshed",
                table: "Seasons",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRefreshed",
                table: "Seasons");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEpisodeRefreshed",
                table: "TvShows",
                type: "TEXT",
                nullable: true);
        }
    }
}
