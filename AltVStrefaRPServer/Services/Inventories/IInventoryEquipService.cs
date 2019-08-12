using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IInventoryEquipService
    {
        Task<InventoryEquipItemResponse> EquipItemAsync(Character character, IInventoryContainer inventory, int playerEquipmentId, int itemToEquip);
        Task<InventoryEquipItemResponse> EquipItemAsync(IInventoryContainer inventory, IPlayerEquipment playerEquipment, int itemToEquipId);
    }
}
