using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Migrations
{
    public partial class SetProperIdInOurDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subtitles_EpisodeId",
                table: "Subtitles");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "Subtitles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Scene",
                table: "Subtitles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Episodes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "Episodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_EpisodeId_Language_Version",
                table: "Subtitles",
                columns: new[] { "EpisodeId", "Language", "Version" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subtitles_EpisodeId_Language_Version",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "Scene",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Episodes");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Subtitles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Episodes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_EpisodeId",
                table: "Subtitles",
                column: "EpisodeId");
        }
    }
}
