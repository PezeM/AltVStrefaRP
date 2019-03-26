using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class AccountRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Password = table.Column<string>(nullable: true),
                    LicenseHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AccountId",
                table: "Characters",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Accounts_AccountId",
                table: "Characters",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Accounts_AccountId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Characters_AccountId",
                table: "Characters");
        }
    }
}
