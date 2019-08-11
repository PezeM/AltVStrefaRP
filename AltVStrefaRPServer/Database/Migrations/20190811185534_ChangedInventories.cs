using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class ChangedInventories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Inventories_EquippedItemsInventoryId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_EquippedItemsInventoryId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "EquippedItemsInventoryId",
                table: "InventoryItems");

            migrationBuilder.AlterColumn<int>(
                name: "MaxSlots",
                table: "Inventories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EquipmentId",
                table: "Characters",
                column: "EquipmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Inventories_EquipmentId",
                table: "Characters",
                column: "EquipmentId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Inventories_EquipmentId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_EquipmentId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Characters");

            migrationBuilder.AddColumn<int>(
                name: "EquippedItemsInventoryId",
                table: "InventoryItems",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxSlots",
                table: "Inventories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_EquippedItemsInventoryId",
                table: "InventoryItems",
                column: "EquippedItemsInventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Inventories_EquippedItemsInventoryId",
                table: "InventoryItems",
                column: "EquippedItemsInventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
