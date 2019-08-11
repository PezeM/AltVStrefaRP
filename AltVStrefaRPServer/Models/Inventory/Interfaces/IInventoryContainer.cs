using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryContainer : ISlotInventory
    {
        Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService, IPlayer player = null);

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
    }
}
