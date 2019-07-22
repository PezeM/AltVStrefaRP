using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class ChangedEquipmentSlotName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slot",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "EquipmentSlot",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentSlot",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "Items",
                nullable: true);
        }
    }
}
