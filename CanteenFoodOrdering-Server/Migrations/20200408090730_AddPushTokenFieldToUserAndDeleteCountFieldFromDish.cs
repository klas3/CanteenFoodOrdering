using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class AddPushTokenFieldToUserAndDeleteCountFieldFromDish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Dishes");

            migrationBuilder.AddColumn<string>(
                name: "PushToken",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PushToken",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Dishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
