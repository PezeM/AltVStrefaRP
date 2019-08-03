using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IInventoryDatabaseService _inventoryDatabaseService;

        public InventoryTransferService(IInventoryDatabaseService inventoryDatabaseService)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
        }

        public async Task TransferItemAsync(PlayerInventoryController source, PlayerInventoryController receiver, InventoryItem itemToTransfer, int quantity)
        {
            if (!source.HasItem(itemToTransfer) || itemToTransfer.Quantity < quantity) return;

            var addItemResponse = await receiver.AddItemAsync(itemToTransfer.Item, quantity, _inventoryDatabaseService).ConfigureAwait(false);
            if (addItemResponse.AnyChangesMade) return;

            var removeItemResponse = await source.RemoveItemAsync(itemToTransfer, addItemResponse.ItemsAddedCount, _inventoryDatabaseService).ConfigureAwait(false);
        }
    }
}
