﻿using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Items.Keys;
using AltVStrefaRPServer.Models.Vehicles;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AltVStrefaRPServer.Database
{
    public class ServerContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }

        // Businesses
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessRank> BusinessesRanks { get; set; }
        public DbSet<BusinessPermissions> BusinessesPermissions { get; set; }
        public DbSet<MechanicBusiness> MechanicBusinesses { get; set; }
        public DbSet<RestaurantBusiness> RestaurantBusinesses { get; set; }

        // Fractions
        public DbSet<Fraction> Fractions { get; set; }
        public DbSet<FractionRank> FractionRanks { get; set; }

        public DbSet<PoliceFraction> PoliceFractions { get; set; }
        public DbSet<SamsFraction> SamsFractions { get; set; }
        public DbSet<TownHallFraction> TownHallFractions { get; set; }

        // Permissions
        public DbSet<FractionPermission> FractionPermissions { get; set; }
        public DbSet<InventoryPermission> InventoryPermissions { get; set; }
        public DbSet<ManageEmployeesPermission> ManageEmployeesPermissions { get; set; }
        public DbSet<ManageRanksPermission> ManageRanksPermissions { get; set; }
        public DbSet<OpenMenuPermission> OpenMenuPermissions { get; set; }
        public DbSet<OpenTaxesPagePermission> OpenTaxesPagePermissions { get; set; }
        public DbSet<TownHallActionsPermission> TownHallActionsPermissions { get; set; }
        public DbSet<VehiclePermission> VehiclePermissions { get; set; }

        // Vehicle shop
        public DbSet<VehicleShop> VehicleShops { get; set; }
        public DbSet<VehiclePrice> VehiclePrices { get; set; }

        // Inventory
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PlayerEquipment> PlayerEquipments { get; set; }
        public DbSet<InventoryContainer> InventoryContainers { get; set; }
        public DbSet<PlayerInventoryContainer> PlayerInventories { get; set; }
        public DbSet<VehicleInventoryContainer> VehicleInventories { get; set; }
        public DbSet<BaseItem> Items { get; set; }
        public DbSet<Equipmentable> EquipmentableItems { get; set; }
        public DbSet<Consumable> ConsumableItems { get; set; }
        public DbSet<DrinkItem> DrinkItems { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<WeaponItem> WeaponItems { get; set; }
        public DbSet<ClothItem> Clothes { get; set; }
        public DbSet<KeyItem> Keys { get; set; }
        public DbSet<HouseKeyItem> HouseKeys { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<DroppedItem> DroppedItems { get; set; }

        // Housing
        public DbSet<Interior> Interiors { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<House> Houses { get; set; }

        public ServerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerContext).Assembly);
        }
    }
}
