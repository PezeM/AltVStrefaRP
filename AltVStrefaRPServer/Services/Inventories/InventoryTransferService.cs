using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using System;
using System.Threading.Tasks;
using IInventoryContainer = AltVStrefaRPServer.Models.Inventory.Interfaces.IInventoryContainer;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly Func<ServerContext> _factory;

        public InventoryTransferService(IInventoryDatabaseService inventoryDatabaseService, Func<ServerContext> factory)
        {
            _inventoryDatabaseService = inventoryDatabaseService;
            _factory = factory;
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

            if (!(await receiverInventory.AddInventoryItemAsync(itemToTransfer, newSlot, _inventoryDatabaseService)).AnyChangesMade)
                return InventoryTransferItemResponse.SlotOccupied;

            await sourceInventory.RemoveItemAsync(itemToTransfer, _inventoryDatabaseService);

            return InventoryTransferItemResponse.ItemTransfered;
        }

        public async Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer sourceInventory, IInventoryContainer receiverInventory,
            int itemId, int newSlot)
        {
            if (!sourceInventory.HasItem(itemId, out var itemToTransfer)) return InventoryTransferItemResponse.ItemNotFound;
            return await TransferItemAsync(sourceInventory, receiverInventory, itemToTransfer, newSlot);
        }

        public async Task<InventorySwapItemResponse> SwapItemAsync(IInventoryContainer inventory, int selectedItemId, int selectedItemSlotId,
            int itemToSwapId, int itemToSwapSlotId, IInventoryContainer inventoryToSwap = null)
        {
            var response = new InventorySwapItemResponse();
            if (!inventory.HasItem(selectedItemId, out var selectedItem) || selectedItem.SlotId != selectedItemSlotId) return response;
            if (inventoryToSwap == null)
            {
                if (!inventory.HasItem(itemToSwapId, out var itemToSwap) || itemToSwap.SlotId != itemToSwapSlotId) return response;
                return await SwapItemAsync(inventory, selectedItem, selectedItemSlotId, itemToSwap, itemToSwapSlotId);
            }
            else
            {
                if (!inventoryToSwap.HasItem(itemToSwapId, out var itemToSwap) || itemToSwap.SlotId != itemToSwapSlotId) return response;
                return await SwapItemAsync(inventory, selectedItem, selectedItemSlotId, itemToSwap, itemToSwapSlotId, inventoryToSwap);
            }
        }

        public async Task<InventorySwapItemResponse> SwapItemAsync(IInventoryContainer inventory, InventoryItem selectedItem, int selectedItemSlotId,
            InventoryItem itemToSwap, int itemToSwapSlotId, IInventoryContainer inventoryToSwap = null)
        {
            var response = new InventorySwapItemResponse();
            if (inventoryToSwap == null)
            {
                return await SwapItemInOneInventoryAsync(inventory, selectedItem, selectedItemSlotId, itemToSwap, itemToSwapSlotId, response);
            }
            else
            {
                return await SwapItemsInMultipleInventoriesAsync(inventory, selectedItem, selectedItemSlotId, itemToSwap, itemToSwapSlotId, inventoryToSwap, response);
            }
        }

        private async Task<InventorySwapItemResponse> SwapItemsInMultipleInventoriesAsync(IInventoryContainer inventory, InventoryItem selectedItem,
            int selectedItemSlotId, InventoryItem itemToSwap, int itemToSwapSlotId, IInventoryContainer inventoryToSwap, InventorySwapItemResponse response)
        {
            using (var context = _factory.Invoke())
            {
                // Swap between inventories
                if (!inventory.HasItem(selectedItem) || selectedItem.SlotId != selectedItemSlotId) return response;
                if (!inventoryToSwap.HasItem(itemToSwap) || itemToSwap.SlotId != itemToSwapSlotId) return response;

                if (!inventory.RemoveItemCompletly(selectedItem))
                {
                    response.Type = InventorySwapItemResponseType.CouldntRemoveItem;
                    return response;
                }
                if (!inventoryToSwap.RemoveItemCompletly(itemToSwap))
                {
                    inventory.AddInventoryItem(selectedItem, selectedItemSlotId);
                    response.Type = InventorySwapItemResponseType.CouldntRemoveItem;
                    return response;
                }

                inventory.AddInventoryItem(itemToSwap, selectedItemSlotId);
                inventoryToSwap.AddInventoryItem(selectedItem, itemToSwapSlotId);

                context.Inventories.Update(inventory as Inventory);
                context.Inventories.Update(inventoryToSwap as Inventory);
                await context.SaveChangesAsync();

                response.SelectedItemNewSlotId = selectedItem.SlotId;
                response.SwappedItemNewSlotId = itemToSwap.SlotId;
                response.Type = InventorySwapItemResponseType.ItemsSwapped;
                return response;
            }
        }

        private async Task<InventorySwapItemResponse> SwapItemInOneInventoryAsync(IInventoryContainer inventory, InventoryItem selectedItem,
            int selectedItemSlotId, InventoryItem itemToSwap, int itemToSwapSlotId, InventorySwapItemResponse response)
        {
            // Swap in one inventory
            if (!inventory.HasItem(selectedItem) || selectedItem.SlotId != selectedItemSlotId) return response;
            if (!inventory.HasItem(itemToSwap) || itemToSwap.SlotId != itemToSwapSlotId) return response;

            selectedItem.SetSlot(itemToSwapSlotId);
            itemToSwap.SetSlot(selectedItemSlotId);
            await inventory.UpdateInventoryAsync(_inventoryDatabaseService);
            response.SelectedItemNewSlotId = selectedItem.SlotId;
            response.SwappedItemNewSlotId = itemToSwap.SlotId;
            response.Type = InventorySwapItemResponseType.ItemsSwapped;
            return response;
        }
    }
}
