using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dal.Migrations
{
    /// <inheritdoc />
    public partial class Theory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "FriendRequests");

            migrationBuilder.AddColumn<string>(
                name: "TheoryId",
                table: "Subjects",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Theories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TheoryId",
                table: "Subjects",
                column: "TheoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverUserId",
                table: "FriendRequests",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderUserId",
                table: "FriendRequests",
                column: "SenderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_ReceiverUserId",
                table: "FriendRequests",
                column: "ReceiverUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_SenderUserId",
                table: "FriendRequests",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Theories_TheoryId",
                table: "Subjects",
                column: "TheoryId",
                principalTable: "Theories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_ReceiverUserId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_SenderUserId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Theories_TheoryId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "Theories");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_TheoryId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_ReceiverUserId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_SenderUserId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "TheoryId",
                table: "Subjects");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "FriendRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "FriendRequests",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequests",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_SenderId",
                table: "FriendRequests",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
