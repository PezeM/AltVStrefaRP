using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IInventoryDatabaseService
    {
        IEnumerable<BaseItem> GetAllItems();
        IEnumerable<InventoryItem> GetAllInventoryItems();
        IEnumerable<DroppedItem> GetAllDroppedItems();
        BaseItem GetItem(int id);
        InventoryItem GetInventoryItem(int id);
        void UpdateItem(BaseItem item);
        Task UpdateItemAsync(BaseItem item);
        Task UpdateInventoryAsync(PlayerInventoryContainer playerInventoryContainer);
        Task UpdateInventoryAsync<TInventory>(TInventory inventoryContainer) where TInventory : Inventory;
        void AddNewItem(BaseItem item);
        Task AddNewItemAsync(BaseItem item);
        Task AddInventoryItemAsync(InventoryItem item);
        Task AddInventoryItemsAsync(List<InventoryItem> newItems);
        Task AddDroppedItemAsync(DroppedItem droppedItem);
        Task RemoveItemAsync(InventoryItem item);
        Task RemoveItemAsync(DroppedItem item);
    }
}
