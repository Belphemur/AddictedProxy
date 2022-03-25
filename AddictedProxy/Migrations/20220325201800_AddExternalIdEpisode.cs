using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class AddExternalIdEpisode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "Episodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Episodes");

        }
    }
}
