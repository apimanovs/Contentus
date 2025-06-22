using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramStatsBot.Migrations
{
    /// <inheritdoc />
    public partial class AddLastEditedChannelIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastEditedChannelId",
                table: "Users",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEditedChannelId",
                table: "Users");
        }
    }
}
