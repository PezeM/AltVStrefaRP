using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Fractions;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Database
{
    public class ServerContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<VehicleModel> Vehicles { get; set; }

        // Businesses
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessRank> BusinessesRanks { get; set; }
        public DbSet<BusinessPermissions> BusinessesPermissions { get; set; }
        public DbSet<MechanicBusiness> MechanicBusinesses { get; set; }
        public DbSet<RestaurantBusiness> RestaurantBusinesses { get; set; }

        // Fractions
        public DbSet<Fraction> Fractions { get; set; }
        public DbSet<PoliceFraction> PoliceFractions { get; set; }
        public DbSet<SamsFraction> SamsFractions { get; set; }
        public DbSet<TownHallFraction> TownHallFractions { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }

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
                .HasOne<Business>(c => c.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(c => c.CurrentBusinessId);
                //.OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<Character>()
                    .HasOne<Fraction>(c => c.Fraction)
                    .WithMany(f => f.Employees)
                    .HasForeignKey(c => c.CurrentFractionId);
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
                .Ignore(f => f.Blip);

            modelBuilder.Entity<Fraction>()
                .HasMany<Character>(f => f.Employees)
                .WithOne(c => c.Fraction)
                .HasForeignKey(c => c.CurrentFractionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
