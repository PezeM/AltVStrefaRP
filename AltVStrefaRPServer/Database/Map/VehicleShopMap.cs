using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class VehicleShopMap : IEntityTypeConfiguration<VehicleShop>
    {
        public void Configure(EntityTypeBuilder<VehicleShop> builder)
        {
            builder.Ignore(p => p.ShopBlip);

            builder.HasMany(v => v.AvailableVehicles)
                .WithOne();
        }
    }
}
