using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class InventoryContainerMap : IEntityTypeConfiguration<InventoryContainer>
    {
        public void Configure(EntityTypeBuilder<InventoryContainer> builder)
        {

        }
    }
}
