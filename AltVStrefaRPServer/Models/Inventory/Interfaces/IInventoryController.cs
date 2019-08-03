using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public interface IInventoryController
    {
        int Id { get; }
        int MaxSlots { get; }
        IReadOnlyCollection<InventoryItem> Items { get; }
        bool HasEmptySlots();
        bool HasItem(int id, out InventoryItem item);
        bool HasItem<TItem>() where TItem : BaseItem;
        InventoryItem GetInventoryItem(int itemId);
        bool TryGetInventoryItemNotFullyStacked(BaseItem item, out InventoryItem inventoryItem);
        Task<AddItemResponse> AddItemAsync(BaseItem itemToAdd, int amount, IInventoryDatabaseService inventoryDatabaseService, IPlayer player = null);
    }
}