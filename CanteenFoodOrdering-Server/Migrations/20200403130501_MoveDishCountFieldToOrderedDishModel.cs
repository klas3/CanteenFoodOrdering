using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class MoveDishCountFieldToOrderedDishModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "DishHistories");

            migrationBuilder.AddColumn<int>(
                name: "DishCount",
                table: "OrderedDishes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DishCount",
                table: "OrderedDishes");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "DishHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
