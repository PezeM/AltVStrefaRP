using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventories
{
    public interface IInventoryEquipService
    {
        Task<InventoryEquipItemResponse> EquipItemAsync(Character character, InventoryContainer inventory, int playerEquipmentId, int itemToEquip);
        Task<InventoryEquipItemResponse> EquipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, int itemToEquipId);

        Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, EquipmentSlot slotId);
        Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, Character character, int playerEquipmentId, int itemToEquipId);
        Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, int itemToUnequipId);
        Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, InventoryItem itemToUnequip);
    }
}
