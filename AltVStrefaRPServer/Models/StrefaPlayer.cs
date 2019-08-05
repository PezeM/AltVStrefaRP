using System;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models
{
    public class StrefaPlayer : Player, IStrefaPlayer
    {
        public int AccountId { get; set; }
        public IInventoryController LastOpenedInventory { get; set; }

        public StrefaPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id) { }
    }
}
