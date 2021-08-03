using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhoIsTheBestBoyAPI.Migrations
{
    public partial class ReplaceAvatarArrayWithAvatarURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Dogs");

            migrationBuilder.AddColumn<string>(
                name: "AvatarName",
                table: "Dogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarName",
                table: "Dogs");

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "Dogs",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
