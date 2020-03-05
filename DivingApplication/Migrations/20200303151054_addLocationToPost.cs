using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class addLocationToPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "LatLng",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatLng",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
