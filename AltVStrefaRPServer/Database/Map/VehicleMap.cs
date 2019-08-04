using AltVStrefaRPServer.Models.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class VehicleMap : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.Ignore(v => v.VehicleHandle)
                .Ignore(v => v.IsJobVehicle);

            builder.HasOne(v => v.Inventory)
                .WithOne(vI => vI.Owner)
                .HasForeignKey<VehicleModel>(v => v.InventoryId);
        }
    }
}
