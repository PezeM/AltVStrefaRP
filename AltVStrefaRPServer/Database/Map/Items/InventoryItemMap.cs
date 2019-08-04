using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class InventoryItemMap : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasOne(i => i.Item)
                .WithOne()
                .HasForeignKey<InventoryItem>(i => i.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
