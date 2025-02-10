using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PG",
                newName: "LastName");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "PG",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "PG",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "AddressText",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "PG",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCode",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "PG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "PG",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PG_DistrictCode",
                table: "PG",
                column: "DistrictCode");

            migrationBuilder.CreateIndex(
                name: "IX_PG_ProvinceCode",
                table: "PG",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_PG_WardCode",
                table: "PG",
                column: "WardCode");

            migrationBuilder.AddForeignKey(
                name: "FK_PG_Districts_DistrictCode",
                table: "PG",
                column: "DistrictCode",
                principalTable: "Districts",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_PG_Provinces_ProvinceCode",
                table: "PG",
                column: "ProvinceCode",
                principalTable: "Provinces",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_PG_Wards_WardCode",
                table: "PG",
                column: "WardCode",
                principalTable: "Wards",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PG_Districts_DistrictCode",
                table: "PG");

            migrationBuilder.DropForeignKey(
                name: "FK_PG_Provinces_ProvinceCode",
                table: "PG");

            migrationBuilder.DropForeignKey(
                name: "FK_PG_Wards_WardCode",
                table: "PG");

            migrationBuilder.DropIndex(
                name: "IX_PG_DistrictCode",
                table: "PG");

            migrationBuilder.DropIndex(
                name: "IX_PG_ProvinceCode",
                table: "PG");

            migrationBuilder.DropIndex(
                name: "IX_PG_WardCode",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "AddressText",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "PG");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PG");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "PG",
                newName: "Name");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "PG",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "PG",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
