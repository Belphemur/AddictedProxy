using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations;

[DbContext(typeof(Context.EntityContext))]
[Migration("20260313103000_AddSeasonPackRangeColumns")]
public class AddSeasonPackRangeColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "RangeEnd",
            table: "SeasonPackSubtitles",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "RangeStart",
            table: "SeasonPackSubtitles",
            type: "integer",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RangeEnd",
            table: "SeasonPackSubtitles");

        migrationBuilder.DropColumn(
            name: "RangeStart",
            table: "SeasonPackSubtitles");
    }
}
