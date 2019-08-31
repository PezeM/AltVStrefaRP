using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Models.Houses
{
    public class Hotel : HouseBuilding
    {
        public ICollection<HotelRoom> HotelRooms { get; private set; }
        public int MaximumNumberOfRooms { get; set; }
        
        public override void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.HouseId = Id;
        }

        public bool TryGetHotelRoom(int roomNumber, out HotelRoom hotelRoom)
        {
            hotelRoom = HotelRooms.FirstOrDefault(h => h.HotelRoomNumber == roomNumber);
            return hotelRoom != null;
        }

        public bool AddNewRoom()
        {
            return true;
        }
    }
}