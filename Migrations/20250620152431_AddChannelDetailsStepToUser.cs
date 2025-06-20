using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramStatsBot.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelDetailsStepToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "ChannelUsername",
                table: "Channels",
                newName: "TargetAudience");

            migrationBuilder.RenameColumn(
                name: "ChannelLink",
                table: "Channels",
                newName: "StylePreference");

            migrationBuilder.AddColumn<int>(
                name: "ChannelDetailsStep",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelDetailsStep",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContentGoal",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamplePosts",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelDetailsStep",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "About",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ChannelDetailsStep",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ContentGoal",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ExamplePosts",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "TargetAudience",
                table: "Channels",
                newName: "ChannelUsername");

            migrationBuilder.RenameColumn(
                name: "StylePreference",
                table: "Channels",
                newName: "ChannelLink");

            migrationBuilder.AddColumn<long>(
                name: "ChannelId",
                table: "Channels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
