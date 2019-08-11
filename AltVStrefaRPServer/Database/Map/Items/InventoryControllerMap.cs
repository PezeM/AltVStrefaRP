﻿using AltVStrefaRPServer.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Items
{
    public class InventoryControllerMap : IEntityTypeConfiguration<InventoryContainer>
    {
        public void Configure(EntityTypeBuilder<InventoryContainer> builder)
        {
            builder.HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InventoryId);

            var inventoryControllerNavigation = builder.Metadata
                .FindNavigation(nameof(InventoryContainer.Items));
            inventoryControllerNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
