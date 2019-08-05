using AltV.Net;

namespace AltVStrefaRPServer.Services.Inventory
{
    public static class ItemAdapters
    {
        public static IMValueBaseAdapter DroppedItemAdapter = new DroppedItemAdapter();
        public static IMValueBaseAdapter InventoryContainerAdapter = new InventoryContainerAdapter();
        public static IMValueBaseAdapter InventoryItemAdapter = new InventoryItemAdapter();
    }
}
