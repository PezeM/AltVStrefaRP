using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AltVStrefaRPServer.Database.Migrations
{
    public partial class Initial : Migration
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
                    BlipModel = table.Column<byte>(nullable: false),
                    BlipName = table.Column<string>(nullable: true),
                    BlipColor = table.Column<byte>(nullable: false),
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
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<int>(nullable: false),
                    OwnerType = table.Column<int>(nullable: false),
                    Model = table.Column<string>(nullable: true),
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
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    BackgroundImage = table.Column<string>(nullable: true),
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
                    CurrentFractionId = table.Column<int>(nullable: true)
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
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Money = table.Column<float>(nullable: false),
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "BusinessesPermissions");

            migrationBuilder.DropTable(
                name: "MoneyTransactions");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "BusinessesRanks");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Fractions");

            migrationBuilder.DropTable(
                name: "Businesses");
        }
    }
}
