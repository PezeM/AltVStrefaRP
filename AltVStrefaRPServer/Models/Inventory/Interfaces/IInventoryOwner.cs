namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IInventoryOwner<TOwner, TIHasInventory> where TOwner : IHasInventory<TIHasInventory> where TIHasInventory : IInventory
    {
        TOwner Owner { get; set; }
    }
}
