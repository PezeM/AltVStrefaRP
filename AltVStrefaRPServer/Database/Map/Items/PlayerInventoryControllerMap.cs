using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class PlayerInventoryControllerMap : IEntityTypeConfiguration<PlayerInventoryController>
    {
        public void Configure(EntityTypeBuilder<PlayerInventoryController> builder)
        {
            builder.Ignore(i => i.EquippedItems);

            builder.HasMany(i => i.EquippedItemsList)
                .WithOne()
                .HasForeignKey("EquippedItemsInventoryId");

            var inventoryControllerEquippedItemsNavigation = builder.Metadata
                .FindNavigation(nameof(PlayerInventoryController.EquippedItemsList));
            inventoryControllerEquippedItemsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
