using AltVStrefaRPServer.Models.Interfaces.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class EquipmentableMap : IEntityTypeConfiguration<Equipmentable>
    {
        public void Configure(EntityTypeBuilder<Equipmentable> builder)
        {
            builder.Property(i => i.EquipmentSlot)
                .HasConversion<int>();
        }
    }
}
