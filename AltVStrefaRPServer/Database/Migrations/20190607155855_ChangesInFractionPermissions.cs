using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class ChangesInFractionPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FractionPermissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FractionPermissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FractionPermissions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FractionPermissions");
        }
    }
}
