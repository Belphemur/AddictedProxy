using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddShowPopularity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShowPopularity",
                columns: table => new
                {
                    TvShowId = table.Column<long>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedCount = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowPopularity", x => new { x.TvShowId, x.Language });
                    table.ForeignKey(
                        name: "FK_ShowPopularity_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowPopularity");
        }
    }
}
