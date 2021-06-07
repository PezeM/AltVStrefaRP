using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventory
    {
        int Id { get; }
        IReadOnlyCollection<InventoryItem> Items { get; }
        bool HasItem(InventoryItem item);
        bool HasItem(int id, out InventoryItem item);
        bool HasItem<TItem>() where TItem : BaseItem;
        InventoryItem GetInventoryItem(int itemId);

        AddItemResponse AddInventoryItem(InventoryItem item);
        bool RemoveItemCompletly(InventoryItem item);
        Task<bool> RemoveItemAsync(InventoryItem item, IInventoryDatabaseService inventoryDatabaseService);
        InventoryRemoveResponse RemoveItem(int id, int amount);
        InventoryRemoveResponse RemoveItem(InventoryItem item, int amount);
        Task<InventoryRemoveResponse> RemoveItemAsync(int itemId, int amount, IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, IInventoryDatabaseService inventoryDatabaseService);

        Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager,
             IInventoryDatabaseService inventoryDatabaseService);

        Task UpdateInventoryAsync(IInventoryDatabaseService inventoryDatabaseService);
        InventoryUseResponse UseItem(Character character, int itemId, int quantity = 1);
        InventoryUseResponse UseItem(Character character, InventoryItem item, int quantity = 1);
        Task<InventoryUseResponse> UseItemAsync(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService, int quantity = 1);
        Task<InventoryUseResponse> UseItemAsync(Character character, int itemId,
            IInventoryDatabaseService inventoryDatabaseService);
    }
}
