using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryContainer : IInventory
    {
        int MaxSlots { get; }
        bool HasEmptySlots();
        bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem);
        int CalculateNumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item);
        int CalculateAmountOfItemsToAdd(BaseItem itemToAdd, int amount);

        AddItemResponse AddItem(BaseItem itemToAdd, int amount);
        Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService);

        InventoryStackResponse StackItem(int itemToStackFromId, int itemToStackId);
        InventoryStackResponse StackItem(InventoryItem itemToStackFrom, InventoryItem itemToStack);

        Task<InventoryStackResponse> StackItemAsync(int itemToStackFromId, int itemToStackId, IInventoryDatabaseService inventoryDatabaseService);
        Task<InventoryStackResponse> StackItemAsync(InventoryItem itemToStackFrom, InventoryItem itemToStack, IInventoryDatabaseService inventoryDatabaseService);
    }
}
