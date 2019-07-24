using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Modules.Networking;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoriesManager
    {
        private ConcurrentDictionary<int, DroppedItem> _droppedItems;
        private ConcurrentDictionary<int, InventoryItem> _items;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly NetworkingManager _networkingManager;

        public InventoriesManager(IInventoryDatabaseService inventoryDatabaseService, NetworkingManager networkingManager)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
            _networkingManager = networkingManager;
            _droppedItems = new ConcurrentDictionary<int, DroppedItem>();
            _items = new ConcurrentDictionary<int, InventoryItem>();

            LoadItems();
            LoadDroppedItems();
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

        private void LoadDroppedItems()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var droppedItem in _inventoryDatabaseService.GetAllDroppedItems())
            {
                _droppedItems.TryAdd(droppedItem.Id, droppedItem);
                _networkingManager.AddNewDroppedItem(droppedItem);
            }
            Alt.Log($"Loaded {_droppedItems.Count} dropped items from databse in {Time.GetTimestampMs() - startTime}ms.");
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems() => _droppedItems.Values;

        public bool TryToGetDroppedItem(int droppedItemId, out DroppedItem droppedItem)
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
            _networkingManager.AddNewDroppedItem(droppedItem);
            return true;
        }

        public bool TryToGetDroppedItem(int networkingEntityId, int droppedItemId, out DroppedItem droppedItem)
        {
            droppedItem = default;
            if (!_networkingManager.DoesNetworkingEntityExists(networkingEntityId)) return false;
            if (!TryToGetDroppedItem(droppedItemId, out droppedItem)) return false; 
            return true;
        }

        public async Task<bool> RemoveDroppedItemAsync(DroppedItem droppedItem, int networkingItemId, int itemsAddedCount)
        {
            if (!_droppedItems.ContainsKey(droppedItem.Id)) return false;
            droppedItem.Count -= itemsAddedCount;
            _networkingManager.DescreaseDroppedItemQuantity(networkingItemId, itemsAddedCount);
            if (droppedItem.Count <= 0)
            {
                if (!_droppedItems.TryRemove(droppedItem.Id, out _)) return false;
                await _inventoryDatabaseService.RemoveItemAsync(droppedItem);
            }

            return true;
        }
    }
}
