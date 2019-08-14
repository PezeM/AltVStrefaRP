using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using IInventoryContainer = AltVStrefaRPServer.Models.Inventory.Interfaces.IInventoryContainer;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IInventoryDatabaseService _inventoryDatabaseService;

        public InventoryTransferService(IInventoryDatabaseService inventoryDatabaseService)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
        }

        public InventoryStackResponse StackItemBetweenInventories(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!source.HasItem(itemToStackFromId, out var itemToStackFrom) || !receiver.HasItem(itemToStackId, out var itemToStack))
                return response;
            
            return source.StackItem(itemToStackFrom, itemToStack);
        }

        public async Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryContainer source, IInventoryContainer receiver, 
            int itemToStackFromId, int itemToStackId)
        {
            var response = new InventoryStackResponse(type: InventoryStackResponseType.ItemsNotFound);
            if (!source.HasItem(itemToStackFromId, out var itemToStackFrom) || !receiver.HasItem(itemToStackId, out var itemToStack))
                return response;
            
            return await source.StackItemAsync(itemToStackFrom, itemToStack, _inventoryDatabaseService);
        }

        public async Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer sourceInventory, IInventoryContainer receiverInventory, 
            InventoryItem itemToTransfer, int newSlot)
        {
            if (!sourceInventory.HasItem(itemToTransfer)) return InventoryTransferItemResponse.ItemNotFound;

            if (!(await sourceInventory.AddInventoryItemAsync(itemToTransfer, newSlot, _inventoryDatabaseService)).AnyChangesMade)
                return InventoryTransferItemResponse.SlotOccupied;

            await receiverInventory.RemoveItemAsync(itemToTransfer, _inventoryDatabaseService);

            return InventoryTransferItemResponse.ItemTransfered;
        }

        public async Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer sourceInventory, IInventoryContainer receiverInventory, 
            int itemId, int newSlot)
        {
            if (!sourceInventory.HasItem(itemId, out var itemToTransfer)) return InventoryTransferItemResponse.ItemNotFound;
            return await TransferItemAsync(sourceInventory, receiverInventory, itemToTransfer, newSlot);
        }
    }
}
