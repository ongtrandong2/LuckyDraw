using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AdaptGiftCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionType",
                table: "GiftConditions");

            migrationBuilder.DropColumn(
                name: "MaxAmount",
                table: "GiftConditions");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "GiftConditions");

            migrationBuilder.DropColumn(
                name: "MinParticipants",
                table: "GiftConditions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConditionType",
                table: "GiftConditions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmount",
                table: "GiftConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmount",
                table: "GiftConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MinParticipants",
                table: "GiftConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
