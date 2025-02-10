using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignToRewardOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardOrders_PG_PgId",
                table: "RewardOrders");

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "RewardOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PGId",
                table: "RewardOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardOrders_CampaignId",
                table: "RewardOrders",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardOrders_PGId",
                table: "RewardOrders",
                column: "PGId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardOrders_Campaigns_CampaignId",
                table: "RewardOrders",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RewardOrders_PG_PGId",
                table: "RewardOrders",
                column: "PGId",
                principalTable: "PG",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardOrders_PG_PgId",
                table: "RewardOrders",
                column: "PgId",
                principalTable: "PG",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardOrders_Campaigns_CampaignId",
                table: "RewardOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RewardOrders_PG_PGId",
                table: "RewardOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RewardOrders_PG_PgId",
                table: "RewardOrders");

            migrationBuilder.DropIndex(
                name: "IX_RewardOrders_CampaignId",
                table: "RewardOrders");

            migrationBuilder.DropIndex(
                name: "IX_RewardOrders_PGId",
                table: "RewardOrders");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "RewardOrders");

            migrationBuilder.DropColumn(
                name: "PGId",
                table: "RewardOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardOrders_PG_PgId",
                table: "RewardOrders",
                column: "PgId",
                principalTable: "PG",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
