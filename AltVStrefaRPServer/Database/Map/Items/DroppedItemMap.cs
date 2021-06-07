using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class DroppedItemMap : IEntityTypeConfiguration<DroppedItem>
    {
        public void Configure(EntityTypeBuilder<DroppedItem> builder)
        {
            builder.HasOne(i => i.Item)
                .WithOne()
                .HasForeignKey<DroppedItem>(i => i.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
