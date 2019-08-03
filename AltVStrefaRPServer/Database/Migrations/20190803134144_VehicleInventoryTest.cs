using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class VehicleInventoryTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleInventoryId",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleInventoryId",
                table: "Vehicles",
                column: "VehicleInventoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Inventories_VehicleInventoryId",
                table: "Vehicles",
                column: "VehicleInventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Inventories_VehicleInventoryId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleInventoryId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleInventoryId",
                table: "Vehicles");
        }
    }
}
