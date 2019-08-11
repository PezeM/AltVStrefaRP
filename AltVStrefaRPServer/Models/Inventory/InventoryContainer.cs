using System;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Models.Inventory
{
    public abstract class InventoryContainer: Inventory, IInventoryContainer
    {
        public int MaxSlots { get; protected set; }

        public bool HasEmptySlots() => _items.Count < MaxSlots;

        protected InventoryContainer(){}

        public InventoryContainer(int maxSlots) : base()
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

        public virtual async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            var response = new AddItemResponse(0, false);

            while (amount > 0)
            {
                if (TryGetInventoryItemNotFullyStacked(itemToAdd, out var item))
                {
                    var toAdd = CalculateNumberOfItemsToAdd(itemToAdd, amount, item);
                    item.AddToQuantity(toAdd);
                    response.ItemsAddedCount += toAdd;
                    amount -= toAdd;
                }
                else
                {
                    if (!HasEmptySlots()) return response;
                    var toAdd = CalculateAmountOfItemsToAdd(itemToAdd, amount);

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
            }

            return response;
        }

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
