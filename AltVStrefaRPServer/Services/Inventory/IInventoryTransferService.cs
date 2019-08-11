using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventory
{
    public interface IInventoryTransferService
    {
        Task<InventoryStackResponse> StackItemBetweenInventoriesAsync(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId, bool saveToDatabse = false);
        Task TransferItemAsync(PlayerInventoryContainer source, PlayerInventoryContainer receiver, InventoryItem itemToTransfer, int quantity);
    }
}
