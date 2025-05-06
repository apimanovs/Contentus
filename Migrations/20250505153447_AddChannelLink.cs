using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramStatsBot.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelLink",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelLink",
                table: "Channels");
        }
    }
}
