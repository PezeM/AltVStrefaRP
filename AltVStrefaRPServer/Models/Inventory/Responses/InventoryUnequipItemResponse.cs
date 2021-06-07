namespace AltVStrefaRPServer.Models.Inventory.Responses
{
    public struct InventoryUnequipItemResponse
    {
        public int UnequipedItemNewSlotId { get; set; }
        public InventoryUnequipItemResponseType Type { get; set; }
    }

    public enum InventoryUnequipItemResponseType
    {
        EquipmentInventoryNotFound,
        ItemNotFound,
        ItemNotEquipmentable,
        NoItemAtThatSlot,
        InventoryIsFull,
        ItemUnequipped,
    }
}
