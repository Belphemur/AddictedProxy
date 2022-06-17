using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddTvShowPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TvShows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql("UPDATE TvShows SET Priority = 10 WHERE TvShows.Name = 'Doctor Who (2005)'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TvShows");
        }
    }
}
