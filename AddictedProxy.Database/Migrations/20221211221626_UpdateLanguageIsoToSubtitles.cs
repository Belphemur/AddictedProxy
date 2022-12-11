﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLanguageIsoToSubtitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCodeIso3Letters",
                table: "Subtitles");

            migrationBuilder.AddColumn<string>(
                name: "LanguageIsoCode",
                table: "Subtitles",
                type: "VARCHAR",
                maxLength: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageIsoCode",
                table: "Subtitles");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCodeIso3Letters",
                table: "Subtitles",
                type: "VARCHAR",
                maxLength: 3,
                nullable: true);
        }
    }
}
