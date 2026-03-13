using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonPackEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeasonPackEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeasonPackSubtitleId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    EpisodeTitle = table.Column<string>(type: "text", nullable: true),
                    ReleaseGroup = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonPackEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonPackEntries_SeasonPackSubtitles_SeasonPackSubtitleId",
                        column: x => x.SeasonPackSubtitleId,
                        principalTable: "SeasonPackSubtitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackEntries_SeasonPackSubtitleId_EpisodeNumber",
                table: "SeasonPackEntries",
                columns: new[] { "SeasonPackSubtitleId", "EpisodeNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackEntries_SeasonPackSubtitleId_FileName",
                table: "SeasonPackEntries",
                columns: new[] { "SeasonPackSubtitleId", "FileName" },
                unique: true);

            migrationBuilder.Sql("""
                                 CREATE TRIGGER updated_at_trigger_seasonpackentries
                                 BEFORE UPDATE ON "SeasonPackEntries"
                                 FOR EACH ROW EXECUTE FUNCTION updated_set_now();
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DROP TRIGGER IF EXISTS updated_at_trigger_seasonpackentries ON "SeasonPackEntries";
                                 """);

            migrationBuilder.DropTable(
                name: "SeasonPackEntries");
        }
    }
}
