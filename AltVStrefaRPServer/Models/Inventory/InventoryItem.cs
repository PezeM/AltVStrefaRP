using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryItem
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
    }
}
