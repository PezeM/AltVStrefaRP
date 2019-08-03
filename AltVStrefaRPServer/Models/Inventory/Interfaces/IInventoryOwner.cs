namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryOwner<TOwner, TIHasInventory> where TOwner : IHasInventory<TIHasInventory> where TIHasInventory : IInventoryController
    {
        TOwner Owner { get; set; }
    }
}
