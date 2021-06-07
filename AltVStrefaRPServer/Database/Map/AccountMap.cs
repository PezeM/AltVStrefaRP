using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class AccountMap : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasMany(c => c.Characters)
                .WithOne(a => a.Account)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.AdminLevel)
                .HasDefaultValue(AdminLevel.None)
                .HasConversion<int>();
        }
    }
}
