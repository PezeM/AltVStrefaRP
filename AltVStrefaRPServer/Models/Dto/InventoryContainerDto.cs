using System.Collections.Generic;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models.Dto
{
    public class InventoryContainerDto
    {
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public int InventorySlots { get; set; }
        public List<InventoryItemDto> Items { get; set; }
    }
}
