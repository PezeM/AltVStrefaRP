using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Database
{
    public class ServerContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Account> Accounts { get; set; }
        //public DbSet<MyVehicle> Vehicles { get; set; }

        public ServerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasMany(c => c.Characters)
                .WithOne(a => a.Account);
        }
    }
}
