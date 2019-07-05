using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public BaseItem Item { get; set; }
        public int BaseItemId { get; set; }
    }
}
