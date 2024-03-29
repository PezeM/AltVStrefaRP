﻿using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using System;
using AltVStrefaRPServer.Models.Houses.Interfaces;

namespace AltVStrefaRPServer.Models
{
    public class StrefaPlayer : Player, IStrefaPlayer
    {
        public int AccountId { get; set; }
        public IInventoryContainer LastOpenedInventory { get; set; }
        
        public int HouseEnterColshape { get; set; }

        public IHouse EnteredHouse { get; set; }

        public StrefaPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id) { }
    }
}
