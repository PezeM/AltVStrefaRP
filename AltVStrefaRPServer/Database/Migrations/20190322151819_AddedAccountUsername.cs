using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class AddedAccountUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseHash",
                table: "Accounts",
                newName: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Accounts",
                newName: "LicenseHash");
        }
    }
}
