using System;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public class StrefaPlayer : Player, IStrefaPlayer
    {
        public int AccountId { get; set; }

        public StrefaPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id) { }
    }
}
