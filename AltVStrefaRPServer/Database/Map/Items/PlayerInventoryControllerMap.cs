using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class PlayerInventoryControllerMap : IEntityTypeConfiguration<PlayerInventoryController>
    {
        public void Configure(EntityTypeBuilder<PlayerInventoryController> builder)
        {
            builder.HasMany(i => i.EquippedItems)
                .WithOne()
                .HasForeignKey("EquippedItemsInventoryId");

            var inventoryControllerEquippedItemsNavigation = builder.Metadata
                .FindNavigation(nameof(PlayerInventoryController.EquippedItems));
            inventoryControllerEquippedItemsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
