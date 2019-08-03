namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IHasInventory<TInventory> where TInventory : IInventoryController
    {
        TInventory Inventory { get; set; }
        int InventoryId { get; set; }
    }
}
