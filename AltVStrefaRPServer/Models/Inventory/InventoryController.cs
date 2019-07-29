using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Interfaces.Items;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryController : IInventory
    {
        public int Id { get; private set; }
        public int MaxSlots { get; private set; } = 30;
        public Character Owner { get; private set; }
        public int OwnerId { get; private set; }

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
            if (_equippedItems.Any(i => i.SlotId == (int)equipmentable.EquipmentSlot)) return;
            inventoryItem.Item.UseItem(character);
            _items.Remove(inventoryItem);
            inventoryItem.SetSlot((int)equipmentable.EquipmentSlot);
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

        public async Task<InventoryUseResponse> UseItemAsync(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            item.RemoveQuantity(1);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
                await inventoryDatabaseService.RemoveItemAsync(item);
            }
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, int itemId, IInventoryDatabaseService inventoryDatabaseService)
        {
            var item = GetInventoryItem(itemId);
            if (item != null)
            {
                return await UseItemAsync(character, item, inventoryDatabaseService);
            }
            return InventoryUseResponse.ItemNotFound;
        }

        public async Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager, 
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!(item.Item is IDroppable droppable)) return InventoryDropResponse.ItemNotDroppable;
            if (await RemoveItemAsync(item, amount, inventoryDatabaseService) == InventoryRemoveResponse.NotEnoughItems) return InventoryDropResponse.NotEnoughItems;
            var newBaseItem = BaseItem.ShallowClone(item.Item);
            if (!await inventoriesManager.AddDroppedItemAsync(new DroppedItem(amount, droppable.Model, newBaseItem, position)))
                return InventoryDropResponse.ItemAlreadyDropped;
            return InventoryDropResponse.DroppedItem;
        }
        
        public async Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager, 
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryDropResponse.ItemNotFound;
            return await DropItemAsync(item, amount, position, inventoriesManager, inventoryDatabaseService);
        }

        public async Task<InventoryRemoveResponse> RemoveItemAsync(int id, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(id, out var item)) return InventoryRemoveResponse.ItemNotFound;
            return await RemoveItemAsync(item, amount, inventoryDatabaseService);
        }

        public async Task<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (item.Quantity < amount) return InventoryRemoveResponse.NotEnoughItems;
            item.RemoveQuantity(amount);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
                await inventoryDatabaseService.RemoveItemAsync(item);
                return InventoryRemoveResponse.ItemRemovedCompletly;
            }
            return InventoryRemoveResponse.ItemRemoved;
        }

        public async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService, IPlayer player = null)
        {
            var response = new AddItemResponse(0, false);

            while (amount > 0)
            {
                if (TryToGetInventoryItemWithoutFullStack(itemToAdd, out var item))
                {
                    int toAdd = NumberOfItemsToAdd(itemToAdd, amount, item);
                    item.AddToQuantity(toAdd);
                    response.ItemsAddedCount += toAdd;
                    amount -= toAdd;
                    // Update item quantity
                    if (player != null)
                    {
                        player.EmitLocked("updateInventoryItemQuantity", item.Id, item.Quantity);
                    }
                }
                else
                {
                    if (!HasEmptySlots()) return response;

                    int toAdd = Math.Min(amount, itemToAdd.StackSize);

                    if (response.AddedNewItem)
                    {
                        var newBaseItem = BaseItem.ShallowClone(itemToAdd);
                        var newInventoryItem = new InventoryItem(newBaseItem, toAdd, GetFreeSlot());
                        response.NewItems.Add(newInventoryItem);
                        _items.Add(newInventoryItem);
                    }
                    else
                    {
                        var newInventoryItem = new InventoryItem(itemToAdd, toAdd, GetFreeSlot());
                        response.NewItems.Add(newInventoryItem);
                        _items.Add(newInventoryItem);
                    }

                    amount -= toAdd;
                    response.ItemsAddedCount += toAdd;
                    response.AddedNewItem = true;
                }
            }
            if (response.AddedNewItem)
            {
                await inventoryDatabaseService.UpdateInventoryAsync(this);
                if (player != null)
                {
                    player.EmitLocked("inventoryAddNewItem", response.NewItems);
                }
            }
            return response;
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

        private int NumberOfItemsToAdd(BaseItem itemToAdd, int amount, InventoryItem item)
        {
            int maxQuantity = itemToAdd.StackSize - item.Quantity;
            int toAdd = Math.Min(amount, maxQuantity);
            return toAdd;
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
