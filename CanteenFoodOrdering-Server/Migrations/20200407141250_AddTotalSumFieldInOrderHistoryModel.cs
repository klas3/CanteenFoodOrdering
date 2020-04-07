using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class AddTotalSumFieldInOrderHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "TotalSum",
                table: "OrderHistories",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSum",
                table: "OrderHistories");
        }
    }
}
