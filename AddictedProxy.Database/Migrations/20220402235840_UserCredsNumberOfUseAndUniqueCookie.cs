using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    public partial class UserCredsNumberOfUseAndUniqueCookie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsage",
                table: "AddictedUserCredentials",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Usage",
                table: "AddictedUserCredentials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AddictedUserCredentials_Cookie",
                table: "AddictedUserCredentials",
                column: "Cookie",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AddictedUserCredentials_Cookie",
                table: "AddictedUserCredentials");

            migrationBuilder.DropColumn(
                name: "LastUsage",
                table: "AddictedUserCredentials");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "AddictedUserCredentials");
        }
    }
}
