namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IPlayerEquipment : IInventory
    {
        Character Owner { get; }

        void EquipItem(int itemId);
        void EquipItem(InventoryItem item);
        void UnequipItem(int itemId);
        void UnequipItem(InventoryItem item);
    }
}
