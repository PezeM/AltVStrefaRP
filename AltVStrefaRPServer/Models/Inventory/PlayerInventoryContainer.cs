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
