using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramStatsBot.Migrations
{
    /// <inheritdoc />
    public partial class MakeChannelTitleNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                table: "Channels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramUserId",
                table: "Channels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
