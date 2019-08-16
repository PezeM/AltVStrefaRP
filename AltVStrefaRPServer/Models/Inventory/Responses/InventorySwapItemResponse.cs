namespace AltVStrefaRPServer.Models.Inventory.Responses
{
    public struct InventorySwapItemResponse
    {
        public int SelectedItemNewSlotId { get; set; }
        public int SwappedItemNewSlotId { get; set; }
        public InventorySwapItemResponseType Type { get; set; }
    }

    public enum InventorySwapItemResponseType
    {
        ItemsNotFound,
        CouldntRemoveItem,
        ItemsSwapped,
    }
}
