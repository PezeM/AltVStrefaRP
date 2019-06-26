using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class TownHallPropertyNameChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProperyTax",
                table: "Fractions",
                newName: "PropertyTax");

            migrationBuilder.RenameColumn(
                name: "CanManageEmployess",
                table: "FractionRankPermissions",
                newName: "CanManageEmployees");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PropertyTax",
                table: "Fractions",
                newName: "ProperyTax");

            migrationBuilder.RenameColumn(
                name: "CanManageEmployees",
                table: "FractionRankPermissions",
                newName: "CanManageEmployess");
        }
    }
}
