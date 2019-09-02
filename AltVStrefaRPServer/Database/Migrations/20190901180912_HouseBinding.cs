using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class HouseBinding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_HouseBuildings_Flats_FlatId",
            //    table: "HouseBuildings");

            //migrationBuilder.DropIndex(
            //    name: "IX_HouseBuildings_FlatId",
            //    table: "HouseBuildings");

            //migrationBuilder.DropIndex(
            //    name: "IX_Flats_HouseBuildingId",
            //    table: "Flats");

            //migrationBuilder.DropColumn(
            //    name: "FlatId",
            //    table: "HouseBuildings");

            //migrationBuilder.AddColumn<int>(
            //    name: "HouseBuildingId1",
            //    table: "Flats",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Flats_HouseBuildingId",
            //    table: "Flats",
            //    column: "HouseBuildingId",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HouseBuildingId1",
                table: "Flats",
                column: "HouseBuildingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Flats_HouseBuildings_HouseBuildingId1",
                table: "Flats",
                column: "HouseBuildingId1",
                principalTable: "HouseBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flats_HouseBuildings_HouseBuildingId1",
                table: "Flats");

            migrationBuilder.DropIndex(
                name: "IX_Flats_HouseBuildingId",
                table: "Flats");

            migrationBuilder.DropIndex(
                name: "IX_Flats_HouseBuildingId1",
                table: "Flats");

            migrationBuilder.DropColumn(
                name: "HouseBuildingId1",
                table: "Flats");

            migrationBuilder.AddColumn<int>(
                name: "FlatId",
                table: "HouseBuildings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HouseBuildings_FlatId",
                table: "HouseBuildings",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HouseBuildingId",
                table: "Flats",
                column: "HouseBuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_HouseBuildings_Flats_FlatId",
                table: "HouseBuildings",
                column: "FlatId",
                principalTable: "Flats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
