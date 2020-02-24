using Microsoft.EntityFrameworkCore.Migrations;

namespace DivingApplication.Migrations
{
    public partial class noactiontorestrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageLike_Messages_MessageId",
                table: "UserMessageLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageLike_Users_UserId",
                table: "UserMessageLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageRead_Messages_MessageId",
                table: "UserMessageRead");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageRead_Users_UserId",
                table: "UserMessageRead");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostLike_Posts_PostId",
                table: "UserPostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostLike_Users_UserId",
                table: "UserPostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostSave_Posts_PostId",
                table: "UserPostSave");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostSave_Users_UserId",
                table: "UserPostSave");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Posts",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageLike_Messages_MessageId",
                table: "UserMessageLike",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageLike_Users_UserId",
                table: "UserMessageLike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageRead_Messages_MessageId",
                table: "UserMessageRead",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageRead_Users_UserId",
                table: "UserMessageRead",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostLike_Posts_PostId",
                table: "UserPostLike",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostLike_Users_UserId",
                table: "UserPostLike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostSave_Posts_PostId",
                table: "UserPostSave",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostSave_Users_UserId",
                table: "UserPostSave",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageLike_Messages_MessageId",
                table: "UserMessageLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageLike_Users_UserId",
                table: "UserMessageLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageRead_Messages_MessageId",
                table: "UserMessageRead");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessageRead_Users_UserId",
                table: "UserMessageRead");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostLike_Posts_PostId",
                table: "UserPostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostLike_Users_UserId",
                table: "UserPostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostSave_Posts_PostId",
                table: "UserPostSave");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPostSave_Users_UserId",
                table: "UserPostSave");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageLike_Messages_MessageId",
                table: "UserMessageLike",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageLike_Users_UserId",
                table: "UserMessageLike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageRead_Messages_MessageId",
                table: "UserMessageRead",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessageRead_Users_UserId",
                table: "UserMessageRead",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostLike_Posts_PostId",
                table: "UserPostLike",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostLike_Users_UserId",
                table: "UserPostLike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostSave_Posts_PostId",
                table: "UserPostSave",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostSave_Users_UserId",
                table: "UserPostSave",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
