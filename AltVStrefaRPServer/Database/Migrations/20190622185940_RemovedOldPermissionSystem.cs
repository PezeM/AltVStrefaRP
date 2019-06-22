using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class RemovedOldPermissionSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FractionRankPermissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FractionRankPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CanMakeAdvancedActions = table.Column<bool>(nullable: false),
                    CanManageEmployees = table.Column<bool>(nullable: false),
                    CanManageRanks = table.Column<bool>(nullable: false),
                    CanOpenFractionMenu = table.Column<bool>(nullable: false),
                    FractionRankFK = table.Column<int>(nullable: false),
                    FractionRankId = table.Column<int>(nullable: true),
                    HaveFractionKeys = table.Column<bool>(nullable: false),
                    HaveVehicleKeys = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionRankPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionRankPermissions_FractionRanks_FractionRankId",
                        column: x => x.FractionRankId,
                        principalTable: "FractionRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FractionRankPermissions_FractionRankId",
                table: "FractionRankPermissions",
                column: "FractionRankId");
        }
    }
}
