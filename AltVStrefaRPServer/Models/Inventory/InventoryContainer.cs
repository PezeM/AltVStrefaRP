using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Models.Inventory
{
    public abstract class InventoryContainer : SlotInventory, IInventoryContainer
    {
        protected InventoryContainer() {}

        public InventoryContainer(int maxSlots) : base(maxSlots) {}

        public virtual async Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService, IPlayer player = null)
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
                    // Update item quantity
                    player?.EmitLocked("updateInventoryItemQuantity", item.Id, item.Quantity);
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
                player?.EmitLocked("inventoryAddNewItems", response.NewItems);
            }

            return response;
        }

        public virtual async ValueTask<InventoryRemoveResponse> RemoveItemAsync(int id, int amount, bool saveToDatabase = false, 
            IInventoryDatabaseService inventoryDatabaseService = null)
        {
            if (!HasItem(id, out var item)) return InventoryRemoveResponse.ItemNotFound;
            return await RemoveItemAsync(item, amount, saveToDatabase, inventoryDatabaseService);
        }

        public virtual async ValueTask<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, bool saveToDatabase = false,
            IInventoryDatabaseService inventoryDatabaseService = null)
        {
            if (item.Quantity < amount) return InventoryRemoveResponse.NotEnoughItems;
            item.RemoveQuantity(amount);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);

                if (!saveToDatabase)
                    return InventoryRemoveResponse.ItemRemovedCompletly;

                if (inventoryDatabaseService != null)
                    await inventoryDatabaseService.RemoveItemAsync(item);

                return InventoryRemoveResponse.ItemRemovedCompletly;
            }

            return InventoryRemoveResponse.ItemRemoved;
        }

        public virtual async Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager, 
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryDropResponse.ItemNotFound;
            return await DropItemAsync(item, amount, position, inventoriesManager, inventoryDatabaseService);
        }

        public virtual async Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager, 
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!(item.Item is IDroppable droppable)) return InventoryDropResponse.ItemNotDroppable;
            if (await RemoveItemAsync(item, amount, true, inventoryDatabaseService) == InventoryRemoveResponse.NotEnoughItems) 
                return InventoryDropResponse.NotEnoughItems;

            var newBaseItem = BaseItem.ShallowClone(item.Item);
            if (!await inventoriesManager.AddDroppedItemAsync(new DroppedItem(amount, droppable.Model, newBaseItem, position)))
                return InventoryDropResponse.ItemAlreadyDropped;
            return InventoryDropResponse.DroppedItem;
        }

        public virtual async Task<InventoryStackResponse> StackItemAsync(int itemToStackFromId, int itemToStackId, bool saveToDatabase = false, 
            IInventoryDatabaseService inventoryDatabaseService = null)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!HasItem(itemToStackFromId, out var itemToStackFrom) || !HasItem(itemToStackId, out var itemToStack))
                return response;

            return await StackItemAsync(itemToStackFrom, itemToStack, saveToDatabase, inventoryDatabaseService);
        }

        public virtual async Task<InventoryStackResponse> StackItemAsync(InventoryItem itemToStackFrom, InventoryItem itemToStack,
            bool saveToDatabase = false, IInventoryDatabaseService inventoryDatabaseService = null)
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

            if (await RemoveItemAsync(itemToStackFrom, toAdd, saveToDatabase, inventoryDatabaseService) == InventoryRemoveResponse.NotEnoughItems)
            {
                response.Type = InventoryStackResponseType.ItemsNotFound;
                return response;
            }

            response.AmountOfStackedItems += toAdd;
            itemToStack.AddToQuantity(toAdd);

            return response;
        }
    }
}
