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
            
            String u = "lower(hex(randomblob(16)))"; 
            String v = "substr('89ab',abs(random()) % 4 + 1, 1)";
            var rawquery = "UPDATE Subtitles SET UniqueId = substr("+u+",1,8)||'-'||substr("+u+",9,4)||'-4'||substr("+u+",13,3)||'-'||"+v+"||substr("+u+",17,3)||'-'||substr("+u+",21,12)";
            
            migrationBuilder.Sql(rawquery);
            
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
