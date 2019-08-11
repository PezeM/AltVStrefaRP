using System;
using System.Linq;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class SlotInventory : Inventory, ISlotInventory
    {
        public int MaxSlots { get; protected set; }

        public bool HasEmptySlots() => _items.Count < MaxSlots;

        protected SlotInventory(){}

        public SlotInventory(int maxSlots) : base()
        {
            MaxSlots = maxSlots;
        }

        public bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem)
        {
            inventoryItem = _items.FirstOrDefault(i => i.Item.GetType() == item.GetType() && i.Quantity < item.StackSize);
            return inventoryItem != null;
        }

        public int CalculateNumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item)
        {
            var maxQuantity = itemToAdd.StackSize - item.Quantity;
            return Math.Min(amount, maxQuantity);
        }

        public int CalculateAmountOfItemsToAdd(BaseItem itemToAdd, int amount) => Math.Min(amount, itemToAdd.StackSize);

        protected int GetFreeSlot()
        {
            var freeSlots = Enumerable.Range(0, MaxSlots - 1).ToList();
            for (var i = 0; i < _items.Count; i++)
            {
                if (freeSlots.Contains(_items[i].SlotId))
                {
                    freeSlots.Remove(_items[i].SlotId);
                }
            }
            return freeSlots.First();
        }
    }
}
