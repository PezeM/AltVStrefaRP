using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class InventoryControllerMap : IEntityTypeConfiguration<InventoryController>
    {
        public void Configure(EntityTypeBuilder<InventoryController> builder)
        {
            builder.HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InventoryId);

            var inventoryControllerNavigation = builder.Metadata
                .FindNavigation(nameof(InventoryController.Items));
            inventoryControllerNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
