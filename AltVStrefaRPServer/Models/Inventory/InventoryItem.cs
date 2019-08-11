using AltV.Net;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryItem : IMValueConvertible
    {
        public int Id { get; private set; }
        public int Quantity { get; private set; }
        public BaseItem Item { get; private set; }
        public int SlotId { get; private set; }
        public int BaseItemId { get; protected set; }
        public int InventoryId { get; protected set; }

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

        public IMValueBaseAdapter GetAdapter() => ItemAdapters.InventoryItemAdapter;
    }
}
