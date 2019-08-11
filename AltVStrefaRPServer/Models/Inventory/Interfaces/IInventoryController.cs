using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryController
    {
        int Id { get; }
        int MaxSlots { get; }
        IReadOnlyCollection<InventoryItem> Items { get; }
        bool HasEmptySlots();
        bool HasItem(int id, out InventoryItem item);
        bool HasItem<TItem>() where TItem : BaseItem;
        InventoryItem GetInventoryItem(int itemId);
        bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem);
        Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService, IPlayer player = null);

        InventoryRemoveResponse RemoveItem(int id, int amount);
        InventoryRemoveResponse RemoveItem(InventoryItem item, int amount);
        ValueTask<InventoryRemoveResponse> RemoveItemAsync(int id, int amount, bool saveToDatabase = false, IInventoryDatabaseService inventoryDatabaseService = null);
        ValueTask<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, bool saveToDatabase = false,
            IInventoryDatabaseService inventoryDatabaseService = null);

        Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService);

        Task<InventoryStackResponse> StackItemAsync(int itemToStackFromId, int itemToStackId, bool saveToDatabase = false,
            IInventoryDatabaseService inventoryDatabaseService = null);
        Task<InventoryStackResponse> StackItemAsync(InventoryItem itemToStackFrom, InventoryItem itemToStack, bool saveToDatabase = false,
            IInventoryDatabaseService inventoryDatabaseService = null);

        int CalculateNumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item);
        int CalculateAmountOfItemsToAdd(BaseItem itemToAdd, int amount);
    }
}