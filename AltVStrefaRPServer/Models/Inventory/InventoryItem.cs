using AltV.Net;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryItem : IWritable
    {
        public int Id { get; protected set; }
        public int Quantity { get; protected set; }
        public BaseItem Item { get; protected set; }
        public int SlotId { get; protected set; }
        public int BaseItemId { get; protected set; }

        protected InventoryItem(){}

        public InventoryItem(int id, BaseItem item, int quantity, int slotId) : this(item, quantity, slotId)
        {
            Id = id;
        }

        public InventoryItem(BaseItem item, int quantity, int slotId)
        {
            Item = item;
            Quantity = quantity;
            SlotId = slotId;
        }

        public void AddToQuantity(int items)
        {
            Quantity += items;
        }

        public void RemoveQuantity(int items)
        {
            Quantity -= items;
        }

        public void SetSlot(int slotId)
        {
            SlotId = slotId;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("Id");
            writer.Value(Id);
            writer.Name("Name");
            writer.Value(Item.Name);
            writer.Name("SlotId");
            writer.Value(SlotId);
            writer.Name("Quantity");
            writer.Value(Quantity);
            writer.Name("StackSize");
            writer.Value(Item.StackSize);
            writer.Name("IsDroppable");
            writer.Value((Item is IDroppable));
            writer.Name("EquipmentSlot");
            writer.Value(Item is IEquipmentable equipmentable ? (int)equipmentable.EquipmentSlot : -1);
            writer.EndObject();
        }
    }
}
