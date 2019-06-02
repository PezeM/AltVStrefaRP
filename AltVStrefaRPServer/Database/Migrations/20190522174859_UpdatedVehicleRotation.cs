using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class UpdatedVehicleRotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Heading",
                table: "Vehicles",
                newName: "Yaw");

            migrationBuilder.AddColumn<float>(
                name: "Pitch",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Roll",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pitch",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Roll",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "Yaw",
                table: "Vehicles",
                newName: "Heading");
        }
    }
}
