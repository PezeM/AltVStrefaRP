namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IHasInventory<TInventory> where TInventory : IInventory
    {
        TInventory Inventory { get; set; }
        int InventoryId { get; set; }
    }
}
