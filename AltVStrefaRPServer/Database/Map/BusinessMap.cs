using AltVStrefaRPServer.Models.Businesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class BusinessMap : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.Ignore(b => b.Blip)
                .Property(b => b.Type)
                .HasConversion<int>();

            builder.HasMany(b => b.BusinessRanks)
                .WithOne(c => c.Business)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
