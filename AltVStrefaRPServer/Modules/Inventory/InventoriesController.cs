using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoriesController
    {
        private readonly IInventoryTransferService _inventoryTransferService;

        public InventoriesController(IInventoryTransferService inventoryTransferService)
        {
            _inventoryTransferService = inventoryTransferService;
        }

        public InventoryStackResponse StackItemBetweenInventories(IInventoryContainer source, IInventoryContainer receiver,
            int itemToStackFromId, int itemToStackId)
        {
            return _inventoryTransferService.StackItemBetweenInventories(source, receiver, itemToStackFromId, itemToStackId);
        }
    }
}
