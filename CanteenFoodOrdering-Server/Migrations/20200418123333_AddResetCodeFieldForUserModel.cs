using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class AddResetCodeFieldForUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetCode",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetCode",
                table: "AspNetUsers");
        }
    }
}
