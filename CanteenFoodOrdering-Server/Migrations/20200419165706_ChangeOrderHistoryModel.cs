using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class ChangeOrderHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "DesiredDate",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "TotalSum",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "Wishes",
                table: "OrderHistories");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "OrderHistories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "OrderHistories");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "OrderHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DesiredDate",
                table: "OrderHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "OrderHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "TotalSum",
                table: "OrderHistories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Wishes",
                table: "OrderHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
