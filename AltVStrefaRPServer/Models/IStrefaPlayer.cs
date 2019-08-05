using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models
{
    public interface IStrefaPlayer : IPlayer
    {
        int AccountId { get;set; }
        IInventoryController LastOpenedInventory { get; set; }
    }
}
