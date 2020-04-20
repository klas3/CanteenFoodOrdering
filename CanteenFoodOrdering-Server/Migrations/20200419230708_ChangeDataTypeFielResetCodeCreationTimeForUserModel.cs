using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenFoodOrdering_Server.Migrations
{
    public partial class ChangeDataTypeFielResetCodeCreationTimeForUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ResetCodeCreationTime",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ResetCodeCreationTime",
                table: "AspNetUsers",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime));
        }
    }
}
