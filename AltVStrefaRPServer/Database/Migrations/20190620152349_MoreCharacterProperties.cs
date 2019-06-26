using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class MoreCharacterProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanMakeAdvancedActions",
                table: "FractionRankPermissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanDriveVehicles",
                table: "Characters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Characters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "Characters",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanMakeAdvancedActions",
                table: "FractionRankPermissions");

            migrationBuilder.DropColumn(
                name: "CanDriveVehicles",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "Characters");
        }
    }
}
