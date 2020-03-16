using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class addIntroToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Intro",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Intro",
                table: "Users");
        }
    }
}
