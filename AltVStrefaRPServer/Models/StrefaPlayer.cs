using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using System;

namespace AltVStrefaRPServer.Models
{
    public class StrefaPlayer : Player, IStrefaPlayer
    {
        public int AccountId { get; set; }
        public IInventoryContainer LastOpenedInventory { get; set; }
        
        /// <summary>
        /// Id of house the player is in
        /// </summary>
        public int HouseId { get; set; }
        
        /// <summary>
        /// Id of house which is inside colshape the player is in
        /// </summary>
        public int HouseEnterColshape { get; set; }

        public StrefaPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id) { }
    }
}
