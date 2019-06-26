using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class CascadeDeletingFractionRanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                table: "FractionPermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                table: "FractionPermissions",
                column: "FractionRankId",
                principalTable: "FractionRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                table: "FractionPermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                table: "FractionPermissions",
                column: "FractionRankId",
                principalTable: "FractionRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
