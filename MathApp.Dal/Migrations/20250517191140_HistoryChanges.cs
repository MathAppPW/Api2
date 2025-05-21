using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dal.Migrations
{
    /// <inheritdoc />
    public partial class HistoryChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "UserHistoryEntries");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "UserHistoryEntries");

            migrationBuilder.AddColumn<int>(
                name: "FailedCount",
                table: "UserHistoryEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulCount",
                table: "UserHistoryEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "UserHistoryEntries");

            migrationBuilder.DropColumn(
                name: "SuccessfulCount",
                table: "UserHistoryEntries");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseId",
                table: "UserHistoryEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "UserHistoryEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
