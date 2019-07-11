using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Interfaces.Items;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Services.Inventory;
using net.vieapps.Components.Utility;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryController : IInventory
    {
        public int Id { get; set; }
        public int MaxSlots { get; set; } = 50;
        public Character Owner { get; set; }
        public int OwnerId { get; set; }

        public IReadOnlyCollection<InventoryItem> Items => _items;
        private List<InventoryItem> _items;

        public IReadOnlyCollection<InventoryItem> EquippedItems => _equippedItems;
        private List<InventoryItem> _equippedItems;

        protected InventoryController()
        {
            _items = new List<InventoryItem>();
            _equippedItems = new List<InventoryItem>();
        }

        public InventoryController(int maxSlots)
        {
            _items = new List<InventoryItem>();
            _equippedItems = new List<InventoryItem>();
            MaxSlots = maxSlots;
        }

        public bool HasEmptySlots() => _items.Count < MaxSlots;

        public void TestEquip(Character character, int inventoryItemId)
        {
            if(!HasItem(inventoryItemId, out InventoryItem inventoryItem)) return;
            if (!(inventoryItem.Item is Equipmentable equipmentable)) return;
            if (_equippedItems.Any(i => i.SlotId == (int)equipmentable.Slot)) return;
            inventoryItem.Item.UseItem(character);
            _items.Remove(inventoryItem);
            inventoryItem.SetSlot((int)equipmentable.Slot);
            _equippedItems.Add(inventoryItem);
        }

        public void TestUnequip(Character character, int equippedItemId)
        {
            if(!_equippedItems.Any(i => i.Id == equippedItemId)) return;
            if(!HasEmptySlots()) return;
            var itemToUnequip = _equippedItems.First(i => i.Id == equippedItemId);
            _equippedItems.Remove(itemToUnequip);
            itemToUnequip.SetSlot(GetFreeSlot());
            _items.Add(itemToUnequip);
            character.Player.Emit("unequippedItem", equippedItemId, itemToUnequip.SlotId);
        }

        public async Task<InventoryUseResponse> UseItem(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            item.RemoveQuantity(1);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
                await inventoryDatabaseService.UpdateInventoryAsync(this);
                // Propably save to database if item was removed 
            }
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItem(Character character, int itemId, IInventoryDatabaseService inventoryDatabaseService)
        {
            var item = GetInventoryItem(itemId);
            if (item != null)
            {
                return await UseItem(character, item, inventoryDatabaseService);
            }
            return InventoryUseResponse.ItemNotFound;
        }

        public async Task<InventoryDropResponse> DropItem(InventoryItem item, int amount, Position position, InventoryManager inventoryManager)
        {
            if (!(item.Item is IDroppable droppable)) return InventoryDropResponse.ItemNotDroppable;
            if (RemoveItem(item, amount) == InventoryRemoveResponse.NotEnoughItems) return InventoryDropResponse.NotEnoughItems;
            if (!await inventoryManager.AddDroppedItem(new DroppedItem(item.Item.Id, amount, droppable.Model, item.Item, position)))
                return InventoryDropResponse.ItemAlreadyDropped;
            return InventoryDropResponse.DroppedItem;
        }
        
        public async Task<InventoryDropResponse> DropItem(int itemId, int amount, Position position, InventoryManager inventoryManager)
        {
            if (!HasItem(itemId, out var item)) return InventoryDropResponse.ItemNotFound;
            return await DropItem(item, amount, position, inventoryManager);
        }

        public InventoryRemoveResponse RemoveItem(int id, int amount)
        {
            if (!HasItem(id, out var item)) return InventoryRemoveResponse.ItemNotFound;
            return RemoveItem(item, amount);
        }

        public InventoryRemoveResponse RemoveItem(InventoryItem item, int amount)
        {
            if (item.Quantity < amount) return InventoryRemoveResponse.NotEnoughItems;
            item.RemoveQuantity(amount);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
            }
            return InventoryRemoveResponse.ItemRemoved;
        }

        public async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            int added = 0;
            bool newItemAdded = false;
            while (amount > 0)
            {
                if (TryToGetInventoryItemWithoutFullStack(itemToAdd, out var item))
                {
                    int maxQuantity = itemToAdd.StackSize - item.Quantity;
                    int toAdd = Math.Min(amount, maxQuantity);
                    item.AddToQuantity(toAdd);
                    added += toAdd;
                    amount -= added;
                }
                else
                {
                    if (!HasEmptySlots()) return AddItemResponse.InventoryFull;
                    int toAdd = Math.Min(amount, itemToAdd.StackSize);
                    if (newItemAdded)
                    {
                        //var copy = itemToAdd.ShallowClone();
                        var another = BaseItem.ShallowClone(itemToAdd);
                        Alt.Log($"Copy of item {itemToAdd.GetType().Name} is {itemToAdd.GetType().Name}.");
                        _items.Add(new InventoryItem(another, toAdd, GetFreeSlot()));
                    }
                    else
                    {
                        var newItem = new InventoryItem(itemToAdd, toAdd, GetFreeSlot());
                        _items.Add(newItem);
                    }
                    amount -= toAdd;
                    newItemAdded = true;
                }
            }
            if (newItemAdded)
            {
                await inventoryDatabaseService.UpdateInventoryAsync(this);
            }
            return AddItemResponse.ItemsAdded;
        }

        public bool HasItem(int id, out InventoryItem item)
        {
            //item = _items.FirstOrDefault(i => i.Id == id);
            //return item != null;
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

        public bool TryToGetInventoryItemWithoutFullStack(BaseItem item, out InventoryItem inventoryItem)
        {
            inventoryItem = _items.FirstOrDefault(i => i.Item.GetType() == item.GetType() && i.Quantity < item.StackSize);
            return inventoryItem != null;
        }

        public bool TryToGetEquippedItemAtSlot(EquipmentSlot slot, out InventoryItem item)
        {
            item = _equippedItems.FirstOrDefault(i => i.SlotId == (int)slot);
            return true;
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

        private int GetFreeSlot()
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

        private void Test()
        {
            var item = new EquippedItem<ClothItem>();
        }
    }

    public class InventoryItem<T> where T : BaseItem
    {
        public int Id { get; set; }
        public T Item { get; set; }
        public int Quantity { get; set; }
    }

    public class EquippedItem<T> : InventoryItem<T> where T : Equipmentable
    {
        public EquipmentSlot Slot { get; set; }
    }
}
