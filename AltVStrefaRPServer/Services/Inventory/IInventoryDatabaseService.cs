using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventory
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
        Task UpdateInventoryAsync(InventoryController inventoryController);
        Task AddDroppedItem(DroppedItem droppedItem);
    }
}
