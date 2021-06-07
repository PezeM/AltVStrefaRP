using AltVStrefaRPServer.Models.Fractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class FractionRankMap : IEntityTypeConfiguration<FractionRank>
    {
        public void Configure(EntityTypeBuilder<FractionRank> builder)
        {
            builder.HasMany(p => p.Permissions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.RankType)
                .HasConversion<int>();
        }
    }
}
