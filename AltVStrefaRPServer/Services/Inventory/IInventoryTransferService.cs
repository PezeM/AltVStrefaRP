using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventory
{
    public interface IInventoryTransferService
    {
        Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryController source, IInventoryController receiver,
            int itemToStackFromId, int itemToStackId, bool saveToDatabse = false);
        Task TransferItemAsync(PlayerInventoryController source, PlayerInventoryController receiver, InventoryItem itemToTransfer, int quantity);
    }
}
