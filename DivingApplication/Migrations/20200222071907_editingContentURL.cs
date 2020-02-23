using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class editingContentURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ContentURL",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentURL",
                table: "Posts");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Posts",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
