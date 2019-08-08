using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IInventoryDatabaseService _inventoryDatabaseService;

        public InventoryTransferService(IInventoryDatabaseService inventoryDatabaseService)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
        }

        public async Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryController source, IInventoryController receiver, 
            int itemToStackFromId, int itemToStackId, bool saveToDatabse = false)
        {
            var response = new InventoryStackResponse();
            if (!source.HasItem(itemToStackFromId, out var itemToStackFrom) || !receiver.HasItem(itemToStackId, out var itemToStack))
                return response;

            return await source.StackItemAsync(itemToStackFrom, itemToStack, saveToDatabse, _inventoryDatabaseService);

            //if (!InventoriesHelper.AreItemsStackable(itemToStackFrom, itemToStack)) return InventoryStackResponse.ItemsNotStackable;

            //var toAdd = source.CalculateNumberOfItemsToAdd(itemToStack.Item, itemToStackFrom.Quantity, itemToStack);
            //if (toAdd <= 0) return InventoryStackResponse.ItemsNotStackable;

            //if (await source.RemoveItemAsync(itemToStackFrom, toAdd, _inventoryDatabaseService, true) == InventoryRemoveResponse.NotEnoughItems)
            //    return InventoryStackResponse.ItemsNotFound;

            //itemToStack.AddToQuantity(toAdd);

            //return InventoryStackResponse.ItemsStacked;
        }

        public async Task TransferItemAsync(PlayerInventoryController source, PlayerInventoryController receiver, InventoryItem itemToTransfer, int quantity)
        {
            if (!source.HasItem(itemToTransfer) || itemToTransfer.Quantity < quantity) return;

            var addItemResponse = await receiver.AddItemAsync(itemToTransfer.Item, quantity, _inventoryDatabaseService).ConfigureAwait(false);
            if (addItemResponse.AnyChangesMade) return;

            var removeItemResponse = await source.RemoveItemAsync(itemToTransfer, addItemResponse.ItemsAddedCount, true, _inventoryDatabaseService).ConfigureAwait(false);
        }
    }
}
