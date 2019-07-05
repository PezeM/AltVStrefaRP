using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryController
    {
        public int Id { get; set; }
        public int MaxSlots { get; set; } = 50;
        public IReadOnlyCollection<InventoryItem> Items => _items;
        private List<InventoryItem> _items;

        protected InventoryController()
        {
            _items = new List<InventoryItem>();
        }

        public InventoryController(int maxSlots)
        {
            _items = new List<InventoryItem>();
            MaxSlots = maxSlots;
        }

        public InventoryUseResponse UseItem(Character character, InventoryItem item)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            item.Quantity--;
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
            }
            return InventoryUseResponse.ItemUsed;
        }

        public InventoryUseResponse UseItem(Character character, int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return UseItem(character, item);
            }
            return InventoryUseResponse.ItemNotFound;
        }

        public async Task<InventoryDropResponse> DropItem(InventoryItem item, int amount, Position position, InventoryManager inventoryManager)
        {
            if (!(item.Item is IDroppable droppable)) return InventoryDropResponse.ItemNotDroppable;
            if (RemoveItem(item, amount) == InventoryRemoveResponse.NotEnoughItems) return InventoryDropResponse.NotEnoughItems;
            if (!await inventoryManager.AddDroppedItem(new DroppedItem(item.Id, amount, droppable.Model, item.Item, position)))
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
            item.Quantity -= amount;
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
            }
            return InventoryRemoveResponse.ItemRemoved;
        }

        public async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            int added = 0;
            List<InventoryItem> newItems = new List<InventoryItem>();
            while (amount > 0)
            {
                if (TryToGetItem<BaseItem>(out var item) && item.Quantity < itemToAdd.StackSize)
                {
                    int maxQuantity = itemToAdd.StackSize - item.Quantity;
                    int toAdd = Math.Min(amount, maxQuantity);
                    item.Quantity += toAdd;
                    added += toAdd;
                    amount -= added;
                }
                else
                {
                    if (_items.Count >= MaxSlots)
                    {
                        return AddItemResponse.InventoryFull;
                        // Return message that inventory is ful
                    }
                    else
                    {
                        // What if the amount is more than one item
                        int toAdd = Math.Min(amount, itemToAdd.StackSize);
                        _items.Add(new InventoryItem
                        {
                            Item = itemToAdd,
                            Quantity = toAdd,
                        });
                        amount -= toAdd;
                        newItems.Add(_items.Last());
                        // Save to database to get item id propably
                    }
                }
            }
            await inventoryDatabaseService.UpdateInventoryAsync(this);
            if (newItems.Count > 0)
            {
                // Add to dictionary in inventory manager
            }
            return AddItemResponse.ItemsAdded;
        }

        public void StackItem(InventoryItem item)
        {

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

        public bool TryToGetItem<TItem>(out InventoryItem item) where TItem : BaseItem
        {
            item = _items.FirstOrDefault(i => i.Item is TItem);
            return item != null;
        }
    }
}
