using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class NewItemsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Houses_Interiors_InteriorId",
            //    table: "Houses");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Houses_Characters_OwnerId",
            //    table: "Houses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Houses",
                table: "Houses");

            migrationBuilder.RenameTable(
                name: "Houses",
                newName: "OldHouse");

            migrationBuilder.RenameIndex(
                name: "IX_Houses_OwnerId",
                table: "OldHouse",
                newName: "IX_OldHouse_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Houses_InteriorId",
                table: "OldHouse",
                newName: "IX_OldHouse_InteriorId");

            migrationBuilder.AddColumn<string>(
                name: "LockPattern",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyItem_Model",
                table: "Items",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldHouse",
                table: "OldHouse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldHouse_Interiors_InteriorId",
                table: "OldHouse",
                column: "InteriorId",
                principalTable: "Interiors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OldHouse_Characters_OwnerId",
                table: "OldHouse",
                column: "OwnerId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldHouse_Interiors_InteriorId",
                table: "OldHouse");

            migrationBuilder.DropForeignKey(
                name: "FK_OldHouse_Characters_OwnerId",
                table: "OldHouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldHouse",
                table: "OldHouse");

            migrationBuilder.DropColumn(
                name: "LockPattern",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "KeyItem_Model",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "OldHouse",
                newName: "Houses");

            migrationBuilder.RenameIndex(
                name: "IX_OldHouse_OwnerId",
                table: "Houses",
                newName: "IX_Houses_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_OldHouse_InteriorId",
                table: "Houses",
                newName: "IX_Houses_InteriorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Houses",
                table: "Houses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Houses_Interiors_InteriorId",
                table: "Houses",
                column: "InteriorId",
                principalTable: "Interiors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Houses_Characters_OwnerId",
                table: "Houses",
                column: "OwnerId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
