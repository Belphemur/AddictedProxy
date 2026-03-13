using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperSubtitlesState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SuperSubtitlesState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxSubtitleId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperSubtitlesState", x => x.Id);
                });

            migrationBuilder.Sql("""
                                 CREATE TRIGGER updated_at_trigger_supersubtitlesstate
                                 BEFORE UPDATE ON "SuperSubtitlesState"
                                 FOR EACH ROW EXECUTE FUNCTION updated_set_now();
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DROP TRIGGER IF EXISTS updated_at_trigger_supersubtitlesstate ON "SuperSubtitlesState";
                                 """);

            migrationBuilder.DropTable(
                name: "SuperSubtitlesState");
        }
    }
}
