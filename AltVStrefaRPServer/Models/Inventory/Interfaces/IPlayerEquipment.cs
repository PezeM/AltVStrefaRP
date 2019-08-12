using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IPlayerEquipment : IInventory
    {
        Character Owner { get; }

        InventoryEquipItemResponse EquipItem(InventoryItem item);
        void UnequipItem(int itemId);
        void UnequipItem(InventoryItem item);
    }
}
