﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToOneTimeMigrationRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OneTimeMigrationRelease_MigrationType_State",
                table: "OneTimeMigrationRelease",
                columns: new[] { "MigrationType", "State" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OneTimeMigrationRelease_MigrationType_State",
                table: "OneTimeMigrationRelease");
        }
    }
}
