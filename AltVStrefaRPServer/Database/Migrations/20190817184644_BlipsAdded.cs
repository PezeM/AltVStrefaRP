using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class BlipsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlipColor",
                table: "VehicleShops");

            migrationBuilder.DropColumn(
                name: "BlipSprite",
                table: "VehicleShops");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlipColor",
                table: "VehicleShops",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlipSprite",
                table: "VehicleShops",
                nullable: false,
                defaultValue: 0);
        }
    }
}
