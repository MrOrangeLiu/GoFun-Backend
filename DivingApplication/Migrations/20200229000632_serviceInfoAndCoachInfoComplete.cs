using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class serviceInfoAndCoachInfoComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoachInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    LocationImageUrls = table.Column<string>(nullable: true),
                    SelfieUrls = table.Column<string>(nullable: true),
                    InsturctingLocation = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdateAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachInfos_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    ServiceImageUrls = table.Column<string>(nullable: true),
                    CenterName = table.Column<string>(maxLength: 128, nullable: true),
                    LocalCenterName = table.Column<string>(maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 64, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    Website = table.Column<string>(maxLength: 2048, nullable: true),
                    SocailMedia = table.Column<string>(maxLength: 256, nullable: true),
                    SocialMediaAccount = table.Column<string>(maxLength: 256, nullable: true),
                    CenterOpeningDate = table.Column<DateTime>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    DetailedAddress = table.Column<string>(maxLength: 2048, nullable: true),
                    CenterDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    ServiceCenterType = table.Column<string>(nullable: false),
                    ProvideAccommodation = table.Column<bool>(nullable: false),
                    ProvidServices = table.Column<string>(nullable: true),
                    CenterFacilites = table.Column<string>(nullable: true),
                    DiveAssociations = table.Column<string>(nullable: true),
                    SupportedLanguages = table.Column<string>(nullable: true),
                    SupportedPayment = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceInfos_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachInfos_AuthorId",
                table: "CoachInfos",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceInfos_OwnerId",
                table: "ServiceInfos",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachInfos");

            migrationBuilder.DropTable(
                name: "ServiceInfos");
        }
    }
}
