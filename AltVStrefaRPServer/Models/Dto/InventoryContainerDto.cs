using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Models.Dto
{
    public class InventoryContainerDto : IMValueConvertible
    {
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public int InventorySlots { get; set; }
        public List<InventoryItem> Items { get; set; }

        public IMValueBaseAdapter GetAdapter() => ItemAdapters.InventoryContainerAdapter;
    }
}
