using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonPackEntryUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                table: "SeasonPackEntries",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPackEntries_UniqueId",
                table: "SeasonPackEntries",
                column: "UniqueId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeasonPackEntries_UniqueId",
                table: "SeasonPackEntries");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                table: "SeasonPackEntries");
        }
    }
}
