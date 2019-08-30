using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Models.Houses
{
    public class Hotel : HouseBuilding
    {
        public ICollection<HotelRoom> HotelRooms { get; set; }
        
        public override void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.HouseId = Id;
        }
    }
}