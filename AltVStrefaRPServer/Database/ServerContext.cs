using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
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

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }

        public ServerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasMany(c => c.Characters)
                .WithOne(a => a.Account);

            modelBuilder.Entity<Character>()
                .Ignore(c => c.Player)
                .HasOne(c => c.BankAccount)
                .WithOne(b => b.Character)
                .HasForeignKey<BankAccount>(b => b.CharacterId);

            modelBuilder.Entity<VehicleModel>()
                .Ignore(v => v.VehicleHandle)
                .Ignore(v => v.IsJobVehicle);

            modelBuilder.Entity<MoneyTransaction>()
                .Property(m => m.Type)
                .HasConversion<int>();

            modelBuilder.Entity<Business>()
                .Ignore(b => b.Blip)
                .Property(b => b.Type)
                .HasConversion<int>();

            modelBuilder.Entity<Business>()
                .HasMany(b => b.Employees)
                .WithOne(c => c.Business);

            modelBuilder.Entity<Business>()
                .HasMany(b => b.BusinessRanks)
                .WithOne(c => c.Business)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<BusinessRank>()
            //    .HasOne(b => b.Business)
            //    .WithMany(b => b.BusinessRanks);

            modelBuilder.Entity<BusinessRank>()
                .HasOne(b => b.Permissions)
                .WithOne(r=> r.BusinessRank)
                .HasForeignKey<BusinessPermissions>(b => b.BusinessRankForeignKey);
        }
    }
}
