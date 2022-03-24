using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class AddLastEpisodeRefreshed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastEpisodeRefreshed",
                table: "TvShows",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEpisodeRefreshed",
                table: "TvShows");
        }
    }
}
