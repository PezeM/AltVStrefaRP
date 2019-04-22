using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class BusinessOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Characters",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxMembersCount",
                table: "Businesses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BusinessId",
                table: "Characters",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Businesses_BusinessId",
                table: "Characters",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Businesses_BusinessId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_BusinessId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaxMembersCount",
                table: "Businesses");
        }
    }
}
