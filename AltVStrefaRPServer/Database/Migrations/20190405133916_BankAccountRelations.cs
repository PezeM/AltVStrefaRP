using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class BankAccountRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "Characters");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CharacterId",
                table: "BankAccounts",
                column: "CharacterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Characters_CharacterId",
                table: "BankAccounts",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Characters_CharacterId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CharacterId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<int>(
                name: "BankAccountId",
                table: "Characters",
                nullable: false,
                defaultValue: 0);
        }
    }
}
