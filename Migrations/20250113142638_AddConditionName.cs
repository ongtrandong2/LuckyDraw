using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddConditionName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GiftConditions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GiftConditions");
        }
    }
}
