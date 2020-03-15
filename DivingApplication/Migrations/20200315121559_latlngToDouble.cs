using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class latlngToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatLng",
                table: "Posts");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Posts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Posts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "LatLng",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
