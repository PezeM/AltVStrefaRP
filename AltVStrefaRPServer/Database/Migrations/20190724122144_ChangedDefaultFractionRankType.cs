using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class ChangedDefaultFractionRankType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RankType",
                table: "FractionRanks",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RankType",
                table: "FractionRanks",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int));
        }
    }
}
