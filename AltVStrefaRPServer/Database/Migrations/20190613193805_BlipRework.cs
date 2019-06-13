using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class BlipRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlipModel",
                table: "Fractions");

            migrationBuilder.DropColumn(
                name: "BlipModel",
                table: "Businesses");

            migrationBuilder.AlterColumn<int>(
                name: "BlipSprite",
                table: "Fractions",
                nullable: false,
                oldClrType: typeof(ushort));

            migrationBuilder.AlterColumn<int>(
                name: "BlipColor",
                table: "Fractions",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<int>(
                name: "BlipColor",
                table: "Businesses",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AddColumn<int>(
                name: "BlipSprite",
                table: "Businesses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlipSprite",
                table: "Businesses");

            migrationBuilder.AlterColumn<ushort>(
                name: "BlipSprite",
                table: "Fractions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<byte>(
                name: "BlipColor",
                table: "Fractions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<byte>(
                name: "BlipModel",
                table: "Fractions",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "BlipColor",
                table: "Businesses",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<byte>(
                name: "BlipModel",
                table: "Businesses",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
