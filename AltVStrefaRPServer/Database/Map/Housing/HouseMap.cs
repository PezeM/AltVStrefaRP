using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Housing
{
    public class HouseMap : IEntityTypeConfiguration<House>
    {
        public void Configure(EntityTypeBuilder<House> builder)
        {
            builder.HasOne(h => h.Flat)
                .WithOne()
                .HasForeignKey<Flat>(f => f.HouseBuildingId);
        }
    }
}
