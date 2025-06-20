using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramStatsBot.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelDataToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChannelId",
                table: "Channels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ChannelLink",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannelUsername",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ChannelLink",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ChannelUsername",
                table: "Channels");
        }
    }
}
