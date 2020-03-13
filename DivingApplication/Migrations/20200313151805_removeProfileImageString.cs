using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class removeProfileImageString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
