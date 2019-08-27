using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using System;

namespace AltVStrefaRPServer.Models
{
    public class StrefaPlayer : Player, IStrefaPlayer
    {
        public int AccountId { get; set; }
        public IInventoryContainer LastOpenedInventory { get; set; }
        public int HouseId { get; set; }

        public StrefaPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id) { }
    }
}
