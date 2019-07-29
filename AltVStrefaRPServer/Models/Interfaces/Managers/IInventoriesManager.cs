using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IInventoriesManager : IManager<InventoryItem>
    {
        bool TryToGetDroppedItem(int networkingEntityId, int droppedItemId, out DroppedItem droppedItem);
        bool TryToGetDroppedItem(int droppedItemId, out DroppedItem droppedItem);
        IEnumerable<DroppedItem> GetAllDroppedItems();
        Task<bool> AddDroppedItemAsync(DroppedItem droppedItem);
        Task<bool> RemoveDroppedItemAsync(DroppedItem droppedItem, int networkingItemId, int itemsAddedCount);
    }
}