using System;
using System.Collections.Generic;
using System.Linq;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    //public class Shop
    //{
    //    // Id
    //    // Name
    //    // Position
    //    // PedPosition
    //    // ShopInventory
    //}


    //public class ShopItem
    //{
        
    //}

    //public class ShopInventory
    //{
    //    // Id
    //    // Shop
    //    // List<InventoryItem>
    //    protected List<ShopItem> _items;

    //    public void AddItem()
    //    {
    //        var item = _items[0];
    //        // Add to quantity which is propably infinite or something
    //    }
    //}

    // Inventories in Vehicles/Players/Random boxes/Fraction inventories/Business inventories/Shops etc
    
    public class InventoryController : IInventory
    {
        public int Id { get; protected set; }
        public int MaxSlots { get; protected set; }

        public IReadOnlyCollection<InventoryItem> Items => _items;
        protected List<InventoryItem> _items;

        protected InventoryController()
        {
            _items = new List<InventoryItem>();
        }

        public bool HasEmptySlots() => _items.Count < MaxSlots;

        public bool HasItem(InventoryItem item) => _items.Contains(item);

        public bool HasItem(int id, out InventoryItem item)
        {
            item = null;
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id == id)
                {
                    item = _items[i];
                    return true;
                }
            }
            return item != null;
        }

        public bool HasItem<TItem>() where TItem : BaseItem
        {
            return (_items.FirstOrDefault(i => i.Item is TItem)) != null;
        }

        public InventoryItem GetInventoryItem(int itemId)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id == itemId)
                {
                    return _items[i];
                }
            }
            return null;
        }

        public bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem)
        {
            inventoryItem = _items.FirstOrDefault(i => i.Item.GetType() == item.GetType() && i.Quantity < item.StackSize);
            return inventoryItem != null;
        }

        protected int NumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item)
        {
            int maxQuantity = itemToAdd.StackSize - item.Quantity;
            int toAdd = Math.Min(amount, maxQuantity);
            return toAdd;
        }

        protected int GetFreeSlot()
        {
            var freeSlots = Enumerable.Range(0, MaxSlots - 1).ToList();
            for (int i = 0; i < _items.Count; i++)
            {
                if (freeSlots.Contains(_items[i].SlotId))
                {
                    freeSlots.Remove(_items[i].SlotId);
                }    
            }
            return Enumerable.First(freeSlots);
        }
    }
}
