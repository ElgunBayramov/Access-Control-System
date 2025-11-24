using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.WebUI.Migrations
{
    public partial class PasswordRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPassword",
                schema: "Membership",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPassword",
                schema: "Membership",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
