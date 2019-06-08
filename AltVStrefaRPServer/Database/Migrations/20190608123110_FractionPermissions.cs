using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class FractionPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FractionRanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankName = table.Column<string>(nullable: true),
                    IsDefaultRank = table.Column<bool>(nullable: false),
                    IsHighestRank = table.Column<bool>(nullable: false),
                    FractionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionRanks_Fractions_FractionId",
                        column: x => x.FractionId,
                        principalTable: "Fractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FractionRankPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CanOpenFractionMenu = table.Column<bool>(nullable: false),
                    HaveVehicleKeys = table.Column<bool>(nullable: false),
                    HaveFractionKeys = table.Column<bool>(nullable: false),
                    CanManageRanks = table.Column<bool>(nullable: false),
                    CanManageEmployess = table.Column<bool>(nullable: false),
                    FractionRankFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionRankPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionRankPermissions_FractionRanks_FractionRankFK",
                        column: x => x.FractionRankFK,
                        principalTable: "FractionRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FractionRankPermissions_FractionRankFK",
                table: "FractionRankPermissions",
                column: "FractionRankFK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FractionRanks_FractionId",
                table: "FractionRanks",
                column: "FractionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FractionRankPermissions");

            migrationBuilder.DropTable(
                name: "FractionRanks");
        }
    }
}
