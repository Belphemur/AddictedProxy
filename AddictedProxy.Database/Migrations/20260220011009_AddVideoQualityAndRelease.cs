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

            // PostgreSQL cannot automatically cast text → integer; drop the old text column and add a new integer one.
            migrationBuilder.DropColumn(
                name: "Qualities",
                table: "SeasonPackSubtitles");

            migrationBuilder.AddColumn<int>(
                name: "Qualities",
                table: "SeasonPackSubtitles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
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

            migrationBuilder.DropColumn(
                name: "Qualities",
                table: "SeasonPackSubtitles");

            migrationBuilder.AddColumn<string>(
                name: "Qualities",
                table: "SeasonPackSubtitles",
                type: "text",
                nullable: true);
        }
    }
}
