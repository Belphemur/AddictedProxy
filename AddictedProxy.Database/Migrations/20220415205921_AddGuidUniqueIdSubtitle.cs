using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class AddGuidUniqueIdSubtitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                table: "Subtitles",
                type: "TEXT",
                nullable: true);

            var setGuid = @"UPDATE Subtitles SET UniqueId = (select hex( randomblob(4)) || '-' || hex( randomblob(2))
                          || '-' || '4' || substr( hex( randomblob(2)), 2) || '-'
                          || substr('AB89', 1 + (abs(random()) % 4) , 1)  ||
                          substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)) )";
            migrationBuilder.Sql(setGuid);
            
            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueId",
                table: "Subtitles",
                type: "TEXT",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_UniqueId",
                table: "Subtitles",
                column: "UniqueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subtitles_UniqueId",
                table: "Subtitles");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                table: "Subtitles");
        }
    }
}
