using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class NewBusinessPermisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanKickOldMembers",
                table: "BusinessesPermissions",
                newName: "CanOpenBusinessMenu");

            migrationBuilder.AddColumn<bool>(
                name: "CanManageRanks",
                table: "BusinessesPermissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Transactions",
                table: "Businesses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanManageRanks",
                table: "BusinessesPermissions");

            migrationBuilder.DropColumn(
                name: "Transactions",
                table: "Businesses");

            migrationBuilder.RenameColumn(
                name: "CanOpenBusinessMenu",
                table: "BusinessesPermissions",
                newName: "CanKickOldMembers");
        }
    }
}
