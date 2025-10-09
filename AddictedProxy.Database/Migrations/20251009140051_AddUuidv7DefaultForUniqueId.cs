using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUuidv7DefaultForUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueId",
                table: "TvShows",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueId",
                table: "Subtitles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueId",
                table: "TvShows",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueId",
                table: "Subtitles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");
        }
    }
}
