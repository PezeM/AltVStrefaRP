using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class PlayerEquipmentMap : IEntityTypeConfiguration<PlayerEquipment>
    {
        public void Configure(EntityTypeBuilder<PlayerEquipment> builder)
        {
            builder.Ignore(p => p.EquippedItems);
        }
    }
}
