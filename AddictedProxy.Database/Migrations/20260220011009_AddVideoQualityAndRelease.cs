using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoQualityAndRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Qualities",
                table: "Subtitles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Release",
                table: "Subtitles",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Qualities",
                table: "SeasonPackSubtitles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qualities",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "Release",
                table: "Subtitles");

            migrationBuilder.AlterColumn<string>(
                name: "Qualities",
                table: "SeasonPackSubtitles",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
