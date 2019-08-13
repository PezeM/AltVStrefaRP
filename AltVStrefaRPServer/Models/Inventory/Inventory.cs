using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;

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

        public virtual AddItemResponse AddInventoryItem(InventoryItem item)
        {
            var response = new AddItemResponse(0);
            _items.Add(item);
            response.ItemsAddedCount += item.Quantity;
            response.NewItems.Add(item);

            return response;
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

            await inventoryDatabaseService.UpdateInventoryAsync(this);
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
            if (await RemoveItemAsync(item, amount, inventoryDatabaseService) == InventoryRemoveResponse.NotEnoughItems) 
                return InventoryDropResponse.NotEnoughItems;

            var newBaseItem = BaseItem.ShallowClone(item.Item);
            if (!await inventoriesManager.AddDroppedItemAsync(new DroppedItem(amount, droppable.Model, newBaseItem, position)))
                return InventoryDropResponse.ItemAlreadyDropped;
            return InventoryDropResponse.DroppedItem;
        }
    }
}
