using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryContainer : IInventory
    {
        int MaxSlots { get; }
        bool HasEmptySlots();
        bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem);
        int CalculateNumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item);
        int CalculateAmountOfItemsToAdd(BaseItem itemToAdd, int amount);
    }
}
