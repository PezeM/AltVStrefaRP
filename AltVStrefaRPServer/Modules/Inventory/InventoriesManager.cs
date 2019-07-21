﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
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
            }
            Alt.Log($"Loaded {_droppedItems} dropped items from databse in {Time.GetTimestampMs() - startTime}ms.");
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems() => _droppedItems.Values;

        public async Task<bool> AddDroppedItemAsync(DroppedItem droppedItem)
        {
            await _inventoryDatabaseService.AddDroppedItemAsync(droppedItem);
            if (!_droppedItems.TryAdd(droppedItem.Id, droppedItem))
            {
                // Remove the item
                return false; 
            }
            _networkingManager.AddNewDroppedItem(droppedItem);
            return true;
        }
    }
}