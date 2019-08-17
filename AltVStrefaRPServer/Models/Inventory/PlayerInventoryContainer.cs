using AltV.Net.Async;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerInventoryContainer : InventoryContainer
    {
        public Character Owner { get; set; }
        protected PlayerInventoryContainer() { }

        public PlayerInventoryContainer(int maxSlots) : base(maxSlots)
        {
            MaxSlots = maxSlots;
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
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService,
            int quantity = 1)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            await RemoveItemAsync(item, quantity, inventoryDatabaseService).ConfigureAwait(false);
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, int itemId, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryUseResponse.ItemNotFound;
            return await UseItemAsync(character, item, inventoryDatabaseService);
        }

        protected override void OnNewItemStacked(int itemId, int quantity)
        {
            Owner?.Player?.EmitLocked("updateInventoryItemQuantity", itemId, quantity);
        }

        protected override async Task OnAddedNewItemsAsync(IInventoryDatabaseService inventoryDatabaseService, AddItemResponse response)
        {
            await base.OnAddedNewItemsAsync(inventoryDatabaseService, response).ConfigureAwait(false);
            Owner?.Player?.EmitLocked("inventoryAddNewItems", response.NewItems);
        }
    }
}
