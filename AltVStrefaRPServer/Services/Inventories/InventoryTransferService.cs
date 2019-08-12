using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IInventoryDatabaseService _inventoryDatabaseService;

        public InventoryTransferService(IInventoryDatabaseService inventoryDatabaseService)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
        }

        public async Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryContainer source, IInventoryContainer receiver, 
            int itemToStackFromId, int itemToStackId)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!source.HasItem(itemToStackFromId, out var itemToStackFrom) || !receiver.HasItem(itemToStackId, out var itemToStack))
                return response;

            return await source.StackItemAsync(itemToStackFrom, itemToStack, _inventoryDatabaseService);
        }

        public async Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer source, IInventoryContainer receiver, 
            InventoryItem itemToTransfer, int quantity)
        {
            var response = new InventoryTransferItemResponse();
            if (!source.HasItem(itemToTransfer) || itemToTransfer.Quantity < quantity) return response;

            var addItemResponse = await receiver.AddItemAsync(itemToTransfer.Item, quantity, _inventoryDatabaseService).ConfigureAwait(false);
            if (addItemResponse.AnyChangesMade) return response;

            var removeItemResponse = await source.RemoveItemAsync(itemToTransfer, addItemResponse.ItemsAddedCount, _inventoryDatabaseService).ConfigureAwait(false);
            if (removeItemResponse == InventoryRemoveResponse.ItemNotFound || removeItemResponse == InventoryRemoveResponse.NotEnoughItems)
                return response;

            response.AmountOfItemsTranfered = addItemResponse.ItemsAddedCount;
            return response;
        }
    }
}
