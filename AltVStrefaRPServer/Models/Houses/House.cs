﻿using AltV.Net;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : HouseBuilding
    {
        public Flat Flat { get; set; }
        
        public override void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.HouseId = Id;
        }
    }
}