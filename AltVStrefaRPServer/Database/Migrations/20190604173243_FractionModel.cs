using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class FractionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fractions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Money = table.Column<float>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    BlipModel = table.Column<byte>(nullable: false),
                    BlipName = table.Column<string>(nullable: true),
                    BlipColor = table.Column<byte>(nullable: false),
                    BlipSprite = table.Column<ushort>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    VehicleTax = table.Column<float>(nullable: true),
                    ProperyTax = table.Column<float>(nullable: true),
                    GunTax = table.Column<float>(nullable: true),
                    GlobalTax = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fractions", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CurrentFractionId",
                table: "Characters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CurrentFractionId",
                table: "Characters",
                column: "CurrentFractionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Fractions_CurrentFractionId",
                table: "Characters",
                column: "CurrentFractionId",
                principalTable: "Fractions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentFractionId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Fractions_CurrentFractionId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Fractions");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CurrentFractionId",
                table: "Characters");
        }
    }
}
