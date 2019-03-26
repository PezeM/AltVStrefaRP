using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class CharacterImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Characters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Characters",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimePlayed",
                table: "Characters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TimePlayed",
                table: "Characters");
        }
    }
}
