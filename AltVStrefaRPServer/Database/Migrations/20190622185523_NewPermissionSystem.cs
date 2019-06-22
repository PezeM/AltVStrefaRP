using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class NewPermissionSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FractionRankPermissions_FractionRanks_FractionRankFK",
                table: "FractionRankPermissions");

            migrationBuilder.DropIndex(
                name: "IX_FractionRankPermissions_FractionRankFK",
                table: "FractionRankPermissions");

            migrationBuilder.DropColumn(
                name: "IsDefaultRank",
                table: "FractionRanks");

            migrationBuilder.DropColumn(
                name: "IsHighestRank",
                table: "FractionRanks");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "FractionRanks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RankType",
                table: "FractionRanks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FractionRankId",
                table: "FractionRankPermissions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FractionPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HasPermission = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FractionRankId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                        column: x => x.FractionRankId,
                        principalTable: "FractionRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FractionRankPermissions_FractionRankId",
                table: "FractionRankPermissions",
                column: "FractionRankId");

            migrationBuilder.CreateIndex(
                name: "IX_FractionPermissions_FractionRankId",
                table: "FractionPermissions",
                column: "FractionRankId");

            migrationBuilder.AddForeignKey(
                name: "FK_FractionRankPermissions_FractionRanks_FractionRankId",
                table: "FractionRankPermissions",
                column: "FractionRankId",
                principalTable: "FractionRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FractionRankPermissions_FractionRanks_FractionRankId",
                table: "FractionRankPermissions");

            migrationBuilder.DropTable(
                name: "FractionPermissions");

            migrationBuilder.DropIndex(
                name: "IX_FractionRankPermissions_FractionRankId",
                table: "FractionRankPermissions");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "FractionRanks");

            migrationBuilder.DropColumn(
                name: "RankType",
                table: "FractionRanks");

            migrationBuilder.DropColumn(
                name: "FractionRankId",
                table: "FractionRankPermissions");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultRank",
                table: "FractionRanks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighestRank",
                table: "FractionRanks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FractionRankPermissions_FractionRankFK",
                table: "FractionRankPermissions",
                column: "FractionRankFK",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FractionRankPermissions_FractionRanks_FractionRankFK",
                table: "FractionRankPermissions",
                column: "FractionRankFK",
                principalTable: "FractionRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
