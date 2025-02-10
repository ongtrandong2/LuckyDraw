using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RewardCampaignAddZaloQr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPGQrAccessScanPerUser",
                table: "Campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PGQrAccess",
                table: "Campaigns",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPGQrAccessScanPerUser",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "PGQrAccess",
                table: "Campaigns");
        }
    }
}
