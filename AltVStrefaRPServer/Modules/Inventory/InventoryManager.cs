using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryManager
    {
        private ConcurrentDictionary<int, DroppedItem> _droppedItems;
        private ConcurrentDictionary<int, InventoryItem> _items;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;

        public InventoryManager(IInventoryDatabaseService inventoryDatabaseService)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
            _droppedItems = new ConcurrentDictionary<int, DroppedItem>();
            _items = new ConcurrentDictionary<int, InventoryItem>();

            LoadItems();
        }

        private void LoadItems()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var item in _inventoryDatabaseService.GetAllInventoryItems())
            {
                _items.TryAdd(item.Id, item);
            }
            Alt.Log($"Loaded {_items.Count} items from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems() => _droppedItems.Values;

        public async Task<bool> AddDroppedItem(DroppedItem droppedItem)
        {
            if (!_droppedItems.TryAdd(droppedItem.Id, droppedItem)) return false;
            Alt.EmitAllClients("streamInDroppedItem", droppedItem);
            await _inventoryDatabaseService.AddDroppedItem(droppedItem);
            return true;
        }
    }
}
