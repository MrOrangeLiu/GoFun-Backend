using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class addMessageAndChatRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopic_Posts_PostId",
                table: "PostTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopic_Topics_TopicId",
                table: "PostTopic");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatRoomId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DetailedAddress",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "GroupPhoto",
                table: "ChatRooms");

            migrationBuilder.AlterColumn<string>(
                name: "Intro",
                table: "Users",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "ServiceInfos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "ServiceInfos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "ServiceInfos",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "ServiceInfos",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BelongChatRoomId",
                table: "Messages",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "CoachInfos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "CoachInfos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "CoachInfos",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "CoachInfos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ChatRooms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "GroupProfileImage",
                table: "ChatRooms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "ChatRooms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "ChatRooms",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "ChatRooms",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "ChatRooms",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "ChatRooms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdministrativeArea = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    Locality = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    SubAdministrativeArea = table.Column<string>(nullable: true),
                    SubLocality = table.Column<string>(nullable: true),
                    SubThoroughfare = table.Column<string>(nullable: true),
                    Thoroughfare = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceInfos_PlaceId",
                table: "ServiceInfos",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PlaceId",
                table: "Posts",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_BelongChatRoomId",
                table: "Messages",
                column: "BelongChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachInfos_PlaceId",
                table: "CoachInfos",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_PlaceId",
                table: "ChatRooms",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Places_PlaceId",
                table: "ChatRooms",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CoachInfos_Places_PlaceId",
                table: "CoachInfos",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatRooms_BelongChatRoomId",
                table: "Messages",
                column: "BelongChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Places_PlaceId",
                table: "Posts",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopic_Posts_PostId",
                table: "PostTopic",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopic_Topics_TopicId",
                table: "PostTopic",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceInfos_Places_PlaceId",
                table: "ServiceInfos",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Places_PlaceId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_CoachInfos_Places_PlaceId",
                table: "CoachInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatRooms_BelongChatRoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Places_PlaceId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopic_Posts_PostId",
                table: "PostTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopic_Topics_TopicId",
                table: "PostTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceInfos_Places_PlaceId",
                table: "ServiceInfos");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropIndex(
                name: "IX_ServiceInfos_PlaceId",
                table: "ServiceInfos");

            migrationBuilder.DropIndex(
                name: "IX_Posts_PlaceId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Messages_BelongChatRoomId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_CoachInfos_PlaceId",
                table: "CoachInfos");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_PlaceId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "ServiceInfos");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BelongChatRoomId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "CoachInfos");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "CoachInfos");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "CoachInfos");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "CoachInfos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "GroupProfileImage",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "ChatRooms");

            migrationBuilder.AlterColumn<string>(
                name: "Intro",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailedAddress",
                table: "ServiceInfos",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ServiceInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChatRoomId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ChatRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "GroupPhoto",
                table: "ChatRooms",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopic_Posts_PostId",
                table: "PostTopic",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopic_Topics_TopicId",
                table: "PostTopic",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
