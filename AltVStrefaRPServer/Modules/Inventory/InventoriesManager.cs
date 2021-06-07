using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventories;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Modules.Networking;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoriesManager : IInventoriesManager
    {
        private readonly ConcurrentDictionary<int, DroppedItem> _droppedItems;
        private readonly ConcurrentDictionary<int, InventoryItem> _items;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly ILogger<InventoriesManager> _logger;

        public InventoriesManager(IInventoryDatabaseService inventoryDatabaseService, ILogger<InventoriesManager> logger)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
            _logger = logger;
            _droppedItems = new ConcurrentDictionary<int, DroppedItem>();
            _items = new ConcurrentDictionary<int, InventoryItem>();

            LoadItems();
            LoadDroppedItems();
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems() => _droppedItems.Values;

        public bool TryGetDroppedItem(int droppedItemId, out DroppedItem droppedItem)
            => _droppedItems.TryGetValue(droppedItemId, out droppedItem);

        public async Task<bool> AddDroppedItemAsync(DroppedItem droppedItem)
        {
            await _inventoryDatabaseService.AddDroppedItemAsync(droppedItem);
            if (!_droppedItems.TryAdd(droppedItem.Id, droppedItem))
            {
                // Remove the item
                await _inventoryDatabaseService.RemoveItemAsync(droppedItem);
                return false;
            }
            NetworkingManager.Instance.AddNewDroppedItem(droppedItem);
            return true;
        }

        public bool TryGetDroppedItem(int networkingEntityId, int droppedItemId, out DroppedItem droppedItem)
        {
            droppedItem = null;
            return NetworkingManager.Instance.DoesNetworkingEntityExists(networkingEntityId) && TryGetDroppedItem(droppedItemId, out droppedItem);
        }

        public async Task<bool> RemoveDroppedItemAsync(DroppedItem droppedItem, int networkingItemId, int itemsAddedCount)
        {
            if (!_droppedItems.ContainsKey(droppedItem.Id)) return false;
            droppedItem.Count -= itemsAddedCount;
            NetworkingManager.Instance.DescreaseDroppedItemQuantity(networkingItemId, itemsAddedCount);
            if (droppedItem.Count <= 0)
            {
                if (!_droppedItems.TryRemove(droppedItem.Id, out _)) return false;
                await _inventoryDatabaseService.RemoveItemAsync(droppedItem);
            }

            return true;
        }

        private void LoadItems()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var item in _inventoryDatabaseService.GetAllInventoryItems())
            {
                _items.TryAdd(item.Id, item);
            }
            _logger.LogInformation("Loaded {itemsCount} items from database in {elapsedTime}ms", _items.Count, Time.GetElapsedTime(startTime));
        }

        private void LoadDroppedItems()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var droppedItem in _inventoryDatabaseService.GetAllDroppedItems())
            {
                _droppedItems.TryAdd(droppedItem.Id, droppedItem);
                NetworkingManager.Instance.AddNewDroppedItem(droppedItem);
            }
            _logger.LogInformation("Loaded {droppedItemsCount} dropped items from database in {elapsedTime} ms",
                _droppedItems.Count, Time.GetElapsedTime(startTime));
        }
    }
}
