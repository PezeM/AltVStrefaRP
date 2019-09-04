using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;

namespace AltVStrefaRPServer.Models.Inventory
{
    public abstract class Inventory : IInventory
    {
        public int Id { get; protected set; }

        public IReadOnlyCollection<InventoryItem> Items => _items;
        protected List<InventoryItem> _items;

        protected Inventory()
        {
            _items = new List<InventoryItem>();
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

        public bool HasItem(InventoryItem item) => _items.Contains(item);

        public bool HasItem(int id, out InventoryItem item)
        {
            item = null;
            for (var i = 0; i < _items.Count; i++)
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
            return _items.FirstOrDefault(i => i.Item is TItem) != null;
        }

        public List<TItem> GetItems<TItem>() where TItem : BaseItem
        {
            return _items.Select(i => i.Item).OfType<TItem>().ToList();
        }
        
        public virtual AddItemResponse AddInventoryItem(InventoryItem item)
        {
            var response = new AddItemResponse(0);
            _items.Add(item);
            response.ItemsAddedCount += item.Quantity;
            response.NewItems.Add(item);

            return response;
        }

        public bool RemoveItemCompletly(InventoryItem item)
        {
            return _items.Remove(item);
        }

        public async Task<bool> RemoveItemAsync(InventoryItem item, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!RemoveItemCompletly(item))
                return false;

            await inventoryDatabaseService.UpdateInventoryAsync(this);
            return true;
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
                RemoveItemCompletly(item);
                return InventoryRemoveResponse.ItemRemovedCompletly;
            }

            return InventoryRemoveResponse.ItemRemoved;
        }

        public async Task<InventoryRemoveResponse> RemoveItemAsync(int itemId, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryRemoveResponse.ItemNotFound;
            return await RemoveItemAsync(item, amount, inventoryDatabaseService);
        }

        public async Task<InventoryRemoveResponse> RemoveItemAsync(InventoryItem item, int amount, IInventoryDatabaseService inventoryDatabaseService)
        {
            var response = RemoveItem(item, amount);
            if (response != InventoryRemoveResponse.ItemRemovedCompletly) return response;

            await inventoryDatabaseService.RemoveItemAsync(item);
            //await inventoryDatabaseService.UpdateInventoryAsync(this);
            return InventoryRemoveResponse.ItemRemovedCompletly;
        }

        public async Task<InventoryDropResponse> DropItemAsync(int itemId, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryDropResponse.ItemNotFound;
            return await DropItemAsync(item, amount, position, inventoriesManager, inventoryDatabaseService);
        }

        public async Task<InventoryDropResponse> DropItemAsync(InventoryItem item, int amount, Position position, IInventoriesManager inventoriesManager,
            IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!(item.Item is IDroppable droppable)) return InventoryDropResponse.ItemNotDroppable;
            if (await RemoveItemAsync(item, amount, inventoryDatabaseService).ConfigureAwait(false) == InventoryRemoveResponse.NotEnoughItems)
                return InventoryDropResponse.NotEnoughItems;

            var newBaseItem = BaseItem.ShallowClone(item.Item);
            if (!await inventoriesManager.AddDroppedItemAsync(new DroppedItem(amount, droppable.Model, newBaseItem, position)))
                return InventoryDropResponse.ItemAlreadyDropped;
            return InventoryDropResponse.DroppedItem;
        }

        public async Task UpdateInventoryAsync(IInventoryDatabaseService inventoryDatabaseService)
        {
            await inventoryDatabaseService.UpdateInventoryAsync(this);
        }

        public InventoryUseResponse UseItem(Character character, int itemId, int quantity = 1)
        {
            if (!HasItem(itemId, out var item)) return InventoryUseResponse.ItemNotFound;
            return UseItem(character, item, quantity);
        }

        public InventoryUseResponse UseItem(Character character, InventoryItem item, int quantity = 1)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            RemoveItem(item, quantity);
            character.Player?.EmitLocked("usedItemSuccessfully", Id, item.Id, item.Quantity);
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService,
            int quantity = 1)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            await RemoveItemAsync(item, quantity, inventoryDatabaseService).ConfigureAwait(false);
            character.Player?.EmitLocked("usedItemSuccessfully", Id, item.Id, item.Quantity);
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, int itemId, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryUseResponse.ItemNotFound;
            return await UseItemAsync(character, item, inventoryDatabaseService).ConfigureAwait(false);
        }
    }
}
