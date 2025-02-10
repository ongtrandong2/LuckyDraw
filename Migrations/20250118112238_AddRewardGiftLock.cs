using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardGiftLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CampaignGifts");

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CampaignGifts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "CampaignGifts");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CampaignGifts",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
