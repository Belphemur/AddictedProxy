using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalIdToSubtitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Subtitles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_Source_ExternalId",
                table: "Subtitles",
                columns: new[] { "Source", "ExternalId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subtitles_Source_ExternalId",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Subtitles");
        }
    }
}
