using System.Collections.Generic;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;

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

        InventoryRemoveResponse RemoveItem(int id, int amount);
        InventoryRemoveResponse RemoveItem(InventoryItem item, int amount);
    }
}
