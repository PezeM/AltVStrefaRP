using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using IInventoryContainer = AltVStrefaRPServer.Models.Inventory.Interfaces.IInventoryContainer;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IInventoryTransferService
    {
        InventoryStackResponse StackItemBetweenInventories(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId);
        Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId);
        Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer sourceInventory, IInventoryContainer receiverInventory, int itemId, int newSlot);
        Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer source, IInventoryContainer receiver, InventoryItem itemToTransfer, int newSlot);
        Task<InventorySwapItemResponse> SwapItemAsync(IInventoryContainer inventory, int selectedItemId, int selectedItemSlotId, int itemToSwapId, int itemToSwapSlotId, 
            IInventoryContainer inventoryToSwap);
        Task<InventorySwapItemResponse> SwapItemAsync(IInventoryContainer inventory, InventoryItem selectedItem, int selectedItemSlotId, InventoryItem itemToSwap,
            int itemToSwapSlotId, IInventoryContainer inventoryToSwap = null);
    }
}
