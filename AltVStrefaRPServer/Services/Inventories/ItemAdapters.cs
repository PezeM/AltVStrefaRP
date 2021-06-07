using AltV.Net;

namespace AltVStrefaRPServer.Services.Inventories
{
    public static class ItemAdapters
    {
        public static readonly IMValueBaseAdapter DroppedItemAdapter = new DroppedItemAdapter();
        public static readonly IMValueBaseAdapter InventoryContainerAdapter = new InventoryContainerAdapter();
        public static readonly IMValueBaseAdapter InventoryItemAdapter = new InventoryItemAdapter();
    }
}
