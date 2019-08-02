using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Services.Inventory
{
    public interface IInventoryTransferService
    {
        Task TransferItemAsync(PlayerInventoryController source, PlayerInventoryController receiver, InventoryItem itemToTransfer, int quantity);
    }
}
