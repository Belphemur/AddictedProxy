

#nullable disable

using System;
using AddictedProxy.Database.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace AddictedProxy.Database.Migrations
{
    [DbContext(typeof(EntityContext))]
    [Migration("20220402050310_MoveLastRefreshedToSeason")]
    public partial class MoveLastRefreshedToSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRefreshed",
                table: "Seasons",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastEpisodeRefreshed",
                table: "TvShows",
                type: "TEXT",
                nullable: true);
        }
    }
}
