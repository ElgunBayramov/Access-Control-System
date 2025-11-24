using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.WebUI.Migrations
{
    public partial class DeletedDateAddPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Permissions");
        }
    }
}
