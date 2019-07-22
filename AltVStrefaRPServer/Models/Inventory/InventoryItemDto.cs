using AltVStrefaRPServer.Models.Interfaces.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    //TODO Add serialization
    public class InventoryItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StackSize { get;set; }
        public int Quantity { get;set; }
        public bool IsDroppable { get; set; }
        public int EquipmentSlot { get;set; }
        public int SlotId { get; set; }

        public InventoryItemDto(InventoryItem item)
        {
            Id = item.Id;
            Name = item.Item.Name;
            StackSize = item.Item.StackSize;
            Quantity = item.Quantity;
            IsDroppable = (item.Item is IDroppable);
            EquipmentSlot = item.Item is IEquipmentable equipmentable ? (int)equipmentable.EquipmentSlot : -1;
            SlotId = item.SlotId;
        }
    }
}
