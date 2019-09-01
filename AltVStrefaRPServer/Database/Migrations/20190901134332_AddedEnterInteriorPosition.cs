using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class AddedEnterInteriorPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "EnterX",
                table: "Interiors",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "EnterY",
                table: "Interiors",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "EnterZ",
                table: "Interiors",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterX",
                table: "Interiors");

            migrationBuilder.DropColumn(
                name: "EnterY",
                table: "Interiors");

            migrationBuilder.DropColumn(
                name: "EnterZ",
                table: "Interiors");
        }
    }
}
