using System.Collections.Generic;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public interface IInventory
    {
        int Id { get; }
        int MaxSlots { get; }
        IReadOnlyCollection<InventoryItem> Items { get; }
        bool HasEmptySlots();
        bool HasItem(int id, out InventoryItem item);
        bool HasItem<TItem>() where TItem : BaseItem;
        InventoryItem GetInventoryItem(int itemId);
        bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem);
    }
}