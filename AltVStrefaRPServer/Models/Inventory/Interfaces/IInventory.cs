using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;

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
        InventoryRemoveResponse RemoveItem(int id, int amount);
        InventoryRemoveResponse RemoveItem(InventoryItem item, int amount);
        Task<InventoryRemoveResponse> RemoveItemAsync(int itemId, int amount, IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, IInventoryDatabaseService inventoryDatabaseService);

        Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager,
             IInventoryDatabaseService inventoryDatabaseService);
    }
}
