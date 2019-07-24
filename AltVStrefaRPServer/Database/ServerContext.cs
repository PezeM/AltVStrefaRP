using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Models.Interfaces.Items;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<InventoryController> Inventories { get; set; }
        public DbSet<BaseItem> Items { get; set; }
        public DbSet<Equipmentable> Equipmentables { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<WeaponItem> WeaponItems { get; set; }
        public DbSet<ClothItem> Clothes { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<DroppedItem> DroppedItems { get; set; }

        public ServerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasMany(c => c.Characters)
                .WithOne(a => a.Account)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .Property(a => a.AdminLevel)
                .HasDefaultValue(AdminLevel.None)
                .HasConversion<int>();

            // Character
            modelBuilder.Entity<Character>()
                .Ignore(c => c.Player)
                .HasOne(c => c.BankAccount)
                .WithOne(b => b.Character)
                .HasForeignKey<BankAccount>(b => b.CharacterId);

            modelBuilder.Entity<Character>()
                .Property(p => p.Money)
                .HasField("_money");

            modelBuilder.Entity<Character>()
                .HasOne<Business>(c => c.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(c => c.CurrentBusinessId);
                //.OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Character>()
                .HasOne<Fraction>(c => c.Fraction)
                .WithMany(f => f.Employees)
                .HasForeignKey(c => c.CurrentFractionId);
                //.OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Character>()
                .HasOne(c => c.Inventory)
                .WithOne(i => i.Owner)
                .HasForeignKey<InventoryController>(i => i.OwnerId);
                //.OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<VehicleModel>()
                .Ignore(v => v.VehicleHandle)
                .Ignore(v => v.IsJobVehicle);

            modelBuilder.Entity<MoneyTransaction>()
                .Property(m => m.Type)
                .HasConversion<int>();

            // Businesses
            modelBuilder.Entity<Business>()
                .Ignore(b => b.Blip)
                .Property(b => b.Type)
                .HasConversion<int>();

            modelBuilder.Entity<Business>()
                .HasMany(b => b.BusinessRanks)
                .WithOne(c => c.Business)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BusinessRank>()
                .HasOne(b => b.Permissions)
                .WithOne(r=> r.BusinessRank)
                .HasForeignKey<BusinessPermissions>(b => b.BusinessRankForeignKey);

            // Fractions
            modelBuilder.Entity<Fraction>()
                .Ignore(f => f.Blip)
                .Ignore(f => f.Invites);

            modelBuilder.Entity<Fraction>()
                .HasMany<Character>(f => f.Employees)
                .WithOne(c => c.Fraction)
                .HasForeignKey(c => c.CurrentFractionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            var navigation = modelBuilder.Entity<Fraction>()
                .Metadata.FindNavigation(nameof(Fraction.Employees));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var fractionRankNavigation = modelBuilder.Entity<Fraction>()
                .Metadata.FindNavigation(nameof(Fraction.FractionRanks));
            fractionRankNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Fraction>()
                .HasMany(f => f.FractionRanks)
                .WithOne(fr => fr.Fraction)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FractionRank>()
                .HasMany(p => p.Permissions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FractionRank>()
                .Property(r => r.RankType)
                .HasConversion<int>();

            modelBuilder.Entity<TownHallFraction>()
                .Ignore(q => q.Taxes);

            // Vehicle shop
            modelBuilder.Entity<VehicleShop>()
                .Ignore(p => p.ShopBlip);

            modelBuilder.Entity<VehicleShop>()
                .HasMany(v => v.AvailableVehicles)
                .WithOne();

            modelBuilder.Entity<VehiclePrice>()
                .Property(p => p.VehicleModel)
                .HasConversion<uint>();

            // Items
            modelBuilder.Entity<Equipmentable>()
                .Property(i => i.EquipmentSlot)
                .HasConversion<int>();

            modelBuilder.Entity<WeaponItem>()
                .Property(i => i.WeaponModel)
                .HasConversion<uint>();

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Item)
                .WithOne()
                .HasForeignKey<InventoryItem>(i => i.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryController>()
                .HasMany(i => i.Items)
                .WithOne();

            var inventoryControllerNavigation = modelBuilder.Entity<InventoryController>()
                .Metadata.FindNavigation(nameof(InventoryController.Items));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<InventoryController>()
                .HasMany(i => i.EquippedItems)
                .WithOne();

            var inventoryControllerEquippedItemsNavigation = modelBuilder.Entity<InventoryController>()
                .Metadata.FindNavigation(nameof(InventoryController.EquippedItems));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<DroppedItem>()
                .HasOne(i => i.Item)
                .WithOne()
                .HasForeignKey<DroppedItem>(i => i.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
