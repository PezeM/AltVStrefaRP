using AltVStrefaRPServer.Models.Inventory.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class WeaponItemMap : IEntityTypeConfiguration<WeaponItem>
    {
        public void Configure(EntityTypeBuilder<WeaponItem> builder)
        {
            builder.Property(i => i.WeaponModel)
                .HasConversion<uint>();
        }
    }
}
