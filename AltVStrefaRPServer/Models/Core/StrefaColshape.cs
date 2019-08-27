using System;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models.Core
{
    public class StrefaColshape : ColShape, IStrefaColshape
    {
        public bool HouseColshape { get; set; }
        public int HouseId { get; set; }
        
        public StrefaColshape(IntPtr nativePointer) : base(nativePointer) {}
    }
}