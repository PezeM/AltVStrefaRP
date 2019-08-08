namespace AltVStrefaRPServer.Models.Inventory.Responses
{
    public struct InventoryStackResponse
    {
        public int AmountOfStackedItems { get; set; }
        public InventoryStackResponseType Type { get; set; }

        public InventoryStackResponse(int amountOfStackedItems = 0, InventoryStackResponseType type = InventoryStackResponseType.ItemsStacked)
        {
            AmountOfStackedItems = amountOfStackedItems;
            Type = type;
        }
    }

    public enum InventoryStackResponseType
    {
        ItemsNotFound,
        ItemsStacked,
        ItemsNotStackable,
    }
}
