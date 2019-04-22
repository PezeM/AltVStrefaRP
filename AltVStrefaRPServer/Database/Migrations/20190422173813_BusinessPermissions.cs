using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class BusinessPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessRank",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxRanksCount",
                table: "Businesses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusinessesRanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankName = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessesRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessesRanks_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessesPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HaveCarKeys = table.Column<bool>(nullable: false),
                    HaveBusinessKeys = table.Column<bool>(nullable: false),
                    CanOpenBusinessInventory = table.Column<bool>(nullable: false),
                    CanInviteNewMembers = table.Column<bool>(nullable: false),
                    CanKickOldMembers = table.Column<bool>(nullable: false),
                    BusinessRankForeignKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessesPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessesPermissions_BusinessesRanks_BusinessRankForeignKey",
                        column: x => x.BusinessRankForeignKey,
                        principalTable: "BusinessesRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessesPermissions_BusinessRankForeignKey",
                table: "BusinessesPermissions",
                column: "BusinessRankForeignKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessesRanks_BusinessId",
                table: "BusinessesRanks",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessesPermissions");

            migrationBuilder.DropTable(
                name: "BusinessesRanks");

            migrationBuilder.DropColumn(
                name: "BusinessRank",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaxRanksCount",
                table: "Businesses");
        }
    }
}
