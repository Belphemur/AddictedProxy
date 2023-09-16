using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateAtUpdatedAtForCredsAndOneTimeMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OneTimeMigrationRelease",
                type: "timestamp with time zone",
                nullable: false,
               defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OneTimeMigrationRelease",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AddictedUserCredentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AddictedUserCredentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OneTimeMigrationRelease");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OneTimeMigrationRelease");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AddictedUserCredentials");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AddictedUserCredentials");
        }
    }
}
