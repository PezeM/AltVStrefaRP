using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class VehiclePriceMap : IEntityTypeConfiguration<VehiclePrice>
    {
        public void Configure(EntityTypeBuilder<VehiclePrice> builder)
        {
            builder.Property(p => p.VehicleModel)
                .HasConversion<uint>();
        }
    }
}
