using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiProviderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows");

            migrationBuilder.CreateTable(
                name: "EpisodeExternalIds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeExternalIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeExternalIds_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonPackSubtitles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    TvShowId = table.Column<long>(type: "bigint", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: false),
                    Filename = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    LanguageIsoCode = table.Column<string>(type: "VARCHAR", maxLength: 7, nullable: true),
                    Release = table.Column<string>(type: "text", nullable: true),
                    Uploader = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Qualities = table.Column<string>(type: "text", nullable: true),
                    ReleaseGroups = table.Column<string>(type: "text", nullable: true),
                    StoragePath = table.Column<string>(type: "text", nullable: true),
                    StoredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Discovered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonPackSubtitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonPackSubtitles_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowExternalIds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TvShowId = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowExternalIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowExternalIds_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeExternalIds_EpisodeId_Source",
                table: "EpisodeExternalIds",
                columns: new[] { "EpisodeId", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeExternalIds_Source_ExternalId",
                table: "EpisodeExternalIds",
                columns: new[] { "Source", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackSubtitles_Source_ExternalId",
                table: "SeasonPackSubtitles",
                columns: new[] { "Source", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackSubtitles_TvShowId_Season",
                table: "SeasonPackSubtitles",
                columns: new[] { "TvShowId", "Season" });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackSubtitles_UniqueId",
                table: "SeasonPackSubtitles",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowExternalIds_Source_ExternalId",
                table: "ShowExternalIds",
                columns: new[] { "Source", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowExternalIds_TvShowId_Source",
                table: "ShowExternalIds",
                columns: new[] { "TvShowId", "Source" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpisodeExternalIds");

            migrationBuilder.DropTable(
                name: "SeasonPackSubtitles");

            migrationBuilder.DropTable(
                name: "ShowExternalIds");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows",
                column: "ExternalId",
                unique: true);
        }
    }
}
