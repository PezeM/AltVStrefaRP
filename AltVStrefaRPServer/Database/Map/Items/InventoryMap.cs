using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class InventoryMap : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InventoryId);

            var inventoryControllerNavigation = builder.Metadata
                .FindNavigation(nameof(Inventory.Items));
            inventoryControllerNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
