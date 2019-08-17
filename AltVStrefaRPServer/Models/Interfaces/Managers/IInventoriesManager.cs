using AltVStrefaRPServer.Models.Inventory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IInventoriesManager : IManager<InventoryItem>
    {
        bool TryGetDroppedItem(int networkingEntityId, int droppedItemId, out DroppedItem droppedItem);
        bool TryGetDroppedItem(int droppedItemId, out DroppedItem droppedItem);
        IEnumerable<DroppedItem> GetAllDroppedItems();
        Task<bool> AddDroppedItemAsync(DroppedItem droppedItem);
        Task<bool> RemoveDroppedItemAsync(DroppedItem droppedItem, int networkingItemId, int itemsAddedCount);
    }
}