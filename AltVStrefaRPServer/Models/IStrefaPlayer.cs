using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models
{
    public interface IStrefaPlayer : IPlayer
    {
        int AccountId { get; set; }
        IInventoryContainer LastOpenedInventory { get; set; }
        int HouseId { get; set; }
    }
}
