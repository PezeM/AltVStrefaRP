using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    AdminLevel = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(nullable: false),
                    BusinessName = table.Column<string>(nullable: true),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Money = table.Column<float>(nullable: false),
                    MaxMembersCount = table.Column<int>(nullable: false),
                    MaxRanksCount = table.Column<int>(nullable: false),
                    Transactions = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    BlipName = table.Column<string>(nullable: true),
                    BlipSprite = table.Column<int>(nullable: false),
                    BlipColor = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

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
                    BlipName = table.Column<string>(nullable: true),
                    BlipColor = table.Column<int>(nullable: false),
                    BlipSprite = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    VehicleTax = table.Column<float>(nullable: true),
                    PropertyTax = table.Column<float>(nullable: true),
                    GunTax = table.Column<float>(nullable: true),
                    GlobalTax = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interiors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    EnterX = table.Column<float>(nullable: false),
                    EnterY = table.Column<float>(nullable: false),
                    EnterZ = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interiors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    MaxSlots = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StackSize = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Value = table.Column<ushort>(nullable: true),
                    Consumable_Model = table.Column<string>(nullable: true),
                    EquipmentSlot = table.Column<int>(nullable: true),
                    DrawableId = table.Column<int>(nullable: true),
                    TextureId = table.Column<int>(nullable: true),
                    PaletteId = table.Column<int>(nullable: true),
                    IsProp = table.Column<bool>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    WeaponItem_Model = table.Column<string>(nullable: true),
                    WeaponModel = table.Column<uint>(nullable: true),
                    Ammo = table.Column<int>(nullable: true),
                    KeyItem_Model = table.Column<string>(nullable: true),
                    LockPattern = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoneyTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Source = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Amount = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyTransactions", x => x.Id);
                });

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
                    Money = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleShops", x => x.VehicleShopId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessesRanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsDefaultRank = table.Column<bool>(nullable: false),
                    IsOwnerRank = table.Column<bool>(nullable: false),
                    RankName = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessesRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessesRanks_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FractionRanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankName = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    RankType = table.Column<int>(nullable: false),
                    FractionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionRanks_Fractions_FractionId",
                        column: x => x.FractionId,
                        principalTable: "Fractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    IsBanned = table.Column<bool>(nullable: false),
                    IsMuted = table.Column<bool>(nullable: false),
                    CanDriveVehicles = table.Column<bool>(nullable: false),
                    InventoryId = table.Column<int>(nullable: false),
                    EquipmentId = table.Column<int>(nullable: false),
                    ProfileImage = table.Column<string>(nullable: true),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Dimension = table.Column<short>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Money = table.Column<float>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    LastPlayed = table.Column<DateTime>(nullable: false),
                    TimePlayed = table.Column<int>(nullable: false),
                    CurrentBusinessId = table.Column<int>(nullable: true),
                    BusinessRank = table.Column<int>(nullable: false),
                    CurrentFractionId = table.Column<int>(nullable: true),
                    FractionRank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Businesses_CurrentBusinessId",
                        column: x => x.CurrentBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Fractions_CurrentFractionId",
                        column: x => x.CurrentFractionId,
                        principalTable: "Fractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Inventories_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<int>(nullable: false),
                    OwnerType = table.Column<int>(nullable: false),
                    Model = table.Column<string>(nullable: true),
                    InventoryId = table.Column<int>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Dimension = table.Column<short>(nullable: false),
                    Roll = table.Column<float>(nullable: false),
                    Pitch = table.Column<float>(nullable: false),
                    Yaw = table.Column<float>(nullable: false),
                    MaxFuel = table.Column<float>(nullable: false),
                    Fuel = table.Column<float>(nullable: false),
                    MaxOil = table.Column<float>(nullable: false),
                    Oil = table.Column<float>(nullable: false),
                    Mileage = table.Column<float>(nullable: false),
                    PlateNumber = table.Column<uint>(nullable: false),
                    PlateText = table.Column<string>(nullable: true),
                    IsSpawned = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DroppedItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Count = table.Column<int>(nullable: false),
                    Model = table.Column<string>(nullable: true),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    RemoveTime = table.Column<DateTime>(nullable: false),
                    BaseItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DroppedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DroppedItems_Items_BaseItemId",
                        column: x => x.BaseItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(nullable: false),
                    SlotId = table.Column<int>(nullable: false),
                    BaseItemId = table.Column<int>(nullable: false),
                    InventoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_BaseItemId",
                        column: x => x.BaseItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "BusinessesPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HaveVehicleKeys = table.Column<bool>(nullable: false),
                    HaveBusinessKeys = table.Column<bool>(nullable: false),
                    CanOpenBusinessMenu = table.Column<bool>(nullable: false),
                    CanOpenBusinessInventory = table.Column<bool>(nullable: false),
                    CanInviteNewMembers = table.Column<bool>(nullable: false),
                    CanManageRanks = table.Column<bool>(nullable: false),
                    CanManageEmployess = table.Column<bool>(nullable: false),
                    BusinessRankForeignKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessesPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessesPermissions_BusinessesRanks_BusinessRankForeignKey",
                        column: x => x.BusinessRankForeignKey,
                        principalTable: "BusinessesRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FractionPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HasPermission = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FractionRankId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionPermissions_FractionRanks_FractionRankId",
                        column: x => x.FractionRankId,
                        principalTable: "FractionRanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Money = table.Column<float>(nullable: false),
                    ShowNotificationOnMoneyTransfer = table.Column<bool>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    HouseBuildingId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    HotelRoomNumber = table.Column<int>(nullable: true),
                    HotelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
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
                });

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
                    Discriminator = table.Column<string>(nullable: false),
                    MaximumNumberOfRooms = table.Column<int>(nullable: true),
                    FlatId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseBuildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseBuildings_Flats_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CharacterId",
                table: "BankAccounts",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessesPermissions_BusinessRankForeignKey",
                table: "BusinessesPermissions",
                column: "BusinessRankForeignKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessesRanks_BusinessId",
                table: "BusinessesRanks",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AccountId",
                table: "Characters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CurrentBusinessId",
                table: "Characters",
                column: "CurrentBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CurrentFractionId",
                table: "Characters",
                column: "CurrentFractionId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EquipmentId",
                table: "Characters",
                column: "EquipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_InventoryId",
                table: "Characters",
                column: "InventoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DroppedItems_BaseItemId",
                table: "DroppedItems",
                column: "BaseItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HouseBuildingId",
                table: "Flats",
                column: "HouseBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_InteriorId",
                table: "Flats",
                column: "InteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_OwnerId",
                table: "Flats",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_HotelId",
                table: "Flats",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_FractionPermissions_FractionRankId",
                table: "FractionPermissions",
                column: "FractionRankId");

            migrationBuilder.CreateIndex(
                name: "IX_FractionRanks_FractionId",
                table: "FractionRanks",
                column: "FractionId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseBuildings_FlatId",
                table: "HouseBuildings",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_BaseItemId",
                table: "InventoryItems",
                column: "BaseItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InventoryId",
                table: "InventoryItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePrices_VehicleShopId",
                table: "VehiclePrices",
                column: "VehicleShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InventoryId",
                table: "Vehicles",
                column: "InventoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flats_HouseBuildings_HouseBuildingId",
                table: "Flats",
                column: "HouseBuildingId",
                principalTable: "HouseBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flats_HouseBuildings_HotelId",
                table: "Flats",
                column: "HotelId",
                principalTable: "HouseBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flats_Characters_OwnerId",
                table: "Flats");

            migrationBuilder.DropForeignKey(
                name: "FK_Flats_HouseBuildings_HouseBuildingId",
                table: "Flats");

            migrationBuilder.DropForeignKey(
                name: "FK_Flats_HouseBuildings_HotelId",
                table: "Flats");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "BusinessesPermissions");

            migrationBuilder.DropTable(
                name: "DroppedItems");

            migrationBuilder.DropTable(
                name: "FractionPermissions");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MoneyTransactions");

            migrationBuilder.DropTable(
                name: "VehiclePrices");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "BusinessesRanks");

            migrationBuilder.DropTable(
                name: "FractionRanks");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "VehicleShops");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Fractions");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "HouseBuildings");

            migrationBuilder.DropTable(
                name: "Flats");

            migrationBuilder.DropTable(
                name: "Interiors");
        }
    }
}
