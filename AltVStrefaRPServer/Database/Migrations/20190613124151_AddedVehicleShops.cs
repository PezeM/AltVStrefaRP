using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class AddedVehicleShops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleShops",
                columns: table => new
                {
                    VehicleShopId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    BoughtVehiclesX = table.Column<float>(nullable: false),
                    BoughtVehiclesY = table.Column<float>(nullable: false),
                    BoughtVehiclesZ = table.Column<float>(nullable: false),
                    BoughtVehiclesRoll = table.Column<float>(nullable: false),
                    BoughtVehiclesPitch = table.Column<float>(nullable: false),
                    BoughtVehiclesYaw = table.Column<float>(nullable: false),
                    Money = table.Column<float>(nullable: false),
                    BlipSprite = table.Column<int>(nullable: false),
                    BlipColor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleShops", x => x.VehicleShopId);
                });

            migrationBuilder.CreateTable(
                name: "VehiclePrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<int>(nullable: false),
                    VehicleModel = table.Column<uint>(nullable: false),
                    VehicleShopId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclePrices_VehicleShops_VehicleShopId",
                        column: x => x.VehicleShopId,
                        principalTable: "VehicleShops",
                        principalColumn: "VehicleShopId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePrices_VehicleShopId",
                table: "VehiclePrices",
                column: "VehicleShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehiclePrices");

            migrationBuilder.DropTable(
                name: "VehicleShops");
        }
    }
}
