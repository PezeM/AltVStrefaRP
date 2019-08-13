using System;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Modules.Inventory;
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

        public virtual AddItemResponse AddInventoryItem(InventoryItem item)
        {
            var response = new AddItemResponse(0, false);
            if (!HasEmptySlots()) return response;

            var freeSlot = GetFreeSlot();
            item.SetSlot(freeSlot);
            response.AddedNewItem = true;
            response.NewItems.Add(item);
            response.ItemsAddedCount += item.Quantity;

            return response;
        }

        public virtual AddItemResponse AddItem(BaseItem itemToAdd, int amount)
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
                    OnNewItemStacked(item.Id, item.Quantity);
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
                        AddItem(newInventoryItem);
                    }
                    else
                    {
                        var newInventoryItem = new InventoryItem(itemToAdd, toAdd, GetFreeSlot());
                        response.NewItems.Add(newInventoryItem);
                        AddItem(newInventoryItem);
                    }

                    amount -= toAdd;
                    response.ItemsAddedCount += toAdd;
                    response.AddedNewItem = true;
                }
            }
            return response;
        }

        public virtual async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            var response = AddItem(itemToAdd, amount);
            if (response.AddedNewItem)
            {
                await OnAddedNewItemsAsync(inventoryDatabaseService, response);
            }

            return response;
        }

        protected virtual async Task OnAddedNewItemsAsync(IInventoryDatabaseService inventoryDatabaseService, AddItemResponse response)
        {
            await inventoryDatabaseService.UpdateInventoryAsync(this);
        }
        
        protected virtual void OnNewItemStacked(int itemId, int quantity) {}

        public InventoryStackResponse StackItem(int itemToStackFromId, int itemToStackId)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!HasItem(itemToStackFromId, out var itemToStackFrom) || !HasItem(itemToStackId, out var itemToStack))
                return response;

            return StackItem(itemToStackFrom, itemToStack);
        }

        public InventoryStackResponse StackItem(InventoryItem itemToStackFrom, InventoryItem itemToStack)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsStacked);
            if (!InventoriesHelper.AreItemsStackable(itemToStackFrom, itemToStack))
            {
                response.Type = InventoryStackResponseType.ItemsNotStackable;
                return response;
            }

            var toAdd = CalculateNumberOfItemsToAdd(itemToStack.Item, itemToStackFrom.Quantity, itemToStack);
            if (toAdd <= 0)
            {
                response.Type = InventoryStackResponseType.ItemsNotFound;
                return response;
            }

            if (RemoveItem(itemToStackFrom, toAdd) == InventoryRemoveResponse.NotEnoughItems)
            {
                response.Type = InventoryStackResponseType.ItemsNotFound;
                return response;
            }

            response.AmountOfStackedItems += toAdd;
            itemToStack.AddToQuantity(toAdd);

            return response;
        }

        public virtual async Task<InventoryStackResponse> StackItemAsync(int itemToStackFromId, int itemToStackId, IInventoryDatabaseService inventoryDatabaseService)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!HasItem(itemToStackFromId, out var itemToStackFrom) || !HasItem(itemToStackId, out var itemToStack))
                return response;

            return await StackItemAsync(itemToStackFrom, itemToStack, inventoryDatabaseService);
        }

        public virtual async Task<InventoryStackResponse> StackItemAsync(InventoryItem itemToStackFrom, InventoryItem itemToStack,
            IInventoryDatabaseService inventoryDatabaseService)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsStacked);
            if (!InventoriesHelper.AreItemsStackable(itemToStackFrom, itemToStack))
            {
                response.Type = InventoryStackResponseType.ItemsNotStackable;
                return response;
            }

            var toAdd = CalculateNumberOfItemsToAdd(itemToStack.Item, itemToStackFrom.Quantity, itemToStack);
            if (toAdd <= 0)
            {
                response.Type = InventoryStackResponseType.ItemsNotFound;
                return response;
            }

            if (await RemoveItemAsync(itemToStackFrom, toAdd, inventoryDatabaseService) == InventoryRemoveResponse.NotEnoughItems)
            {
                response.Type = InventoryStackResponseType.ItemsNotFound;
                return response;
            }

            response.AmountOfStackedItems += toAdd;
            itemToStack.AddToQuantity(toAdd);

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
