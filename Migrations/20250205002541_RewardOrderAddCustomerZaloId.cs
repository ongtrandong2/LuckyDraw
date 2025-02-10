using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RewardOrderAddCustomerZaloId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PgId",
                table: "RewardOrders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "CustomerZaloId",
                table: "RewardOrders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerZaloId",
                table: "RewardOrders");

            migrationBuilder.AlterColumn<int>(
                name: "PgId",
                table: "RewardOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
