using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class CountFieldInDishModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Dishes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Dishes");
        }
    }
}
