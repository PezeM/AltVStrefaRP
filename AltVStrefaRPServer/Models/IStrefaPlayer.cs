using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Houses.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Models
{
    public interface IStrefaPlayer : IPlayer
    {
        int AccountId { get; set; }
        IInventoryContainer LastOpenedInventory { get; set; }
        /// <summary>
        /// Id of house which is inside colshape the player is in
        /// </summary>
        int HouseEnterColshape { get; set; }
        /// <summary>
        /// Flat the player is in. Returns null if player is not inside any flat
        /// </summary>
        IHouse EnteredHouse { get; set; }
    }
}
