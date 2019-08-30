using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class ChangedHousingSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HouseBuildings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(nullable: true),
                    InteriorId = table.Column<int>(nullable: false),
                    LockPattern = table.Column<string>(nullable: true),
                    IsLocked = table.Column<bool>(nullable: false),
                    HouseBuildingId1 = table.Column<int>(nullable: true),
                    HouseBuildingId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    HotelRoomNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flats_HouseBuildings_HouseBuildingId",
                        column: x => x.HouseBuildingId,
                        principalTable: "HouseBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flats_HouseBuildings_HouseBuildingId1",
                        column: x => x.HouseBuildingId1,
                        principalTable: "HouseBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flats_Interiors_InteriorId",
                        column: x => x.InteriorId,
                        principalTable: "Interiors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flats_Characters_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flats_HouseBuildings_HouseBuildingId2",
                        column: x => x.HouseBuildingId,
                        principalTable: "HouseBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HouseBuildingId",
                table: "Flats",
                column: "HouseBuildingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HouseBuildingId1",
                table: "Flats",
                column: "HouseBuildingId1");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_InteriorId",
                table: "Flats",
                column: "InteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_OwnerId",
                table: "Flats",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flats");

            migrationBuilder.DropTable(
                name: "HouseBuildings");
        }
    }
}
