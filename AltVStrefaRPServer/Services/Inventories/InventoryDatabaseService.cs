using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryDatabaseService : IInventoryDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public InventoryDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems()
        {
            using (var context = _factory.Invoke())
            {
                return context.DroppedItems
                    .Include(i => i.Item)
                    .ToList();
            }
        }

        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            using (var context = _factory.Invoke())
            {
                return context.InventoryItems
                    .Include(i => i.Item)
                    .ToList();
            }
        }

        public IEnumerable<BaseItem> GetAllItems()
        {
            using (var context = _factory.Invoke())
            {
                return context.Items.ToList();
            }
        }

        public InventoryItem GetInventoryItem(int id)
        {
            using (var context = _factory.Invoke())
            {
                return context.InventoryItems.Find(id);
            }
        }

        public BaseItem GetItem(int id)
        {
            using (var context = _factory.Invoke())
            {
                return context.Items.Find(id);
            }
        }

        public void UpdateItem(BaseItem item)
        {
            using (var context = _factory.Invoke())
            {
                context.Items.Update(item);
                context.SaveChanges();
            }
        }

        public async Task UpdateItemAsync(BaseItem item)
        {
            using (var context = _factory.Invoke())
            {
                context.Items.Update(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateInventoryAsync(PlayerInventoryContainer playerInventoryContainer)
        {
            using (var context = _factory.Invoke())
            {
                context.Inventories.Update(playerInventoryContainer);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateInventoryAsync<TInventory>(TInventory inventoryContainer) where TInventory : Inventory
        {
            using (var context = _factory.Invoke())
            {
                context.Inventories.Update(inventoryContainer);
                await context.SaveChangesAsync();
            }
        }

        public void AddNewItem(BaseItem item)
        {
            using (var context = _factory.Invoke())
            {
                context.Items.Add(item);
                context.SaveChanges();
            }
        }

        public async Task AddNewItemAsync(BaseItem item)
        {
            using (var context = _factory.Invoke())
            {
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddInventoryItemAsync(InventoryItem item)
        {
            using (var context = _factory.Invoke())
            {
                await context.InventoryItems.AddAsync(item).ConfigureAwait(false);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddInventoryItemsAsync(List<InventoryItem> inventoryItems)
        {
            using (var context = _factory.Invoke())
            {
                await context.InventoryItems.AddRangeAsync(inventoryItems).ConfigureAwait(false);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddDroppedItemAsync(DroppedItem droppedItem)
        {
            using (var context = _factory.Invoke())
            {
                await context.DroppedItems.AddAsync(droppedItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(InventoryItem item)
        {
            using (var context = _factory.Invoke())
            {
                context.InventoryItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(DroppedItem item)
        {
            using (var context = _factory.Invoke())
            {
                context.DroppedItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
