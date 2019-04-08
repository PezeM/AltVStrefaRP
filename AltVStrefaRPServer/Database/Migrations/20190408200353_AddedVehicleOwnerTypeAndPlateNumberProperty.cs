using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class AddedVehicleOwnerTypeAndPlateNumberProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsJobVehicle",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlateNumber",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlateText",
                table: "Vehicles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PlateText",
                table: "Vehicles");

            migrationBuilder.AddColumn<bool>(
                name: "IsJobVehicle",
                table: "Vehicles",
                nullable: false,
                defaultValue: false);
        }
    }
}
