using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class FractionsPermissions : Migration
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
                    FractionRankId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionRankPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionRankPermissions_FractionRanks_FractionRankId",
                        column: x => x.FractionRankId,
                        principalTable: "FractionRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FractionPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HasPermission = table.Column<bool>(nullable: false),
                    FractionRankPermissionsId = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionPermissions_FractionRankPermissions_FractionRankPerm~",
                        column: x => x.FractionRankPermissionsId,
                        principalTable: "FractionRankPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FractionPermissions_FractionRankPermissionsId",
                table: "FractionPermissions",
                column: "FractionRankPermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_FractionRankPermissions_FractionRankId",
                table: "FractionRankPermissions",
                column: "FractionRankId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FractionRanks_FractionId",
                table: "FractionRanks",
                column: "FractionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FractionPermissions");

            migrationBuilder.DropTable(
                name: "FractionRankPermissions");

            migrationBuilder.DropTable(
                name: "FractionRanks");
        }
    }
}
