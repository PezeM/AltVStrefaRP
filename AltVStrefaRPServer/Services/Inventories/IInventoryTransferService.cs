using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IInventoryTransferService
    {
        Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId);
        Task<InventoryTransferItemResponse> TransferItemAsync(IInventoryContainer source, IInventoryContainer receiver, InventoryItem itemToTransfer, int quantity);
    }
}
