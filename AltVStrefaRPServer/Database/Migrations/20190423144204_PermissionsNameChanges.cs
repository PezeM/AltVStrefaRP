using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class PermissionsNameChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HaveCarKeys",
                table: "BusinessesPermissions", 
                newName: "HaveVehicleKeys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HaveVehicleKeys",
                table: "BusinessesPermissions", 
                newName: "HaveCarKeys");
        }
    }
}
