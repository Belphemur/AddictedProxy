using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddUpperCaseSubtitleUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Subtitles SET UniqueId = upper(UniqueId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
