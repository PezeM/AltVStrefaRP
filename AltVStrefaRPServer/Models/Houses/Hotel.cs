using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class Hotel : HouseBuilding, IWritable
    {
        public ICollection<HotelRoom> HotelRooms { get; set; }
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

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("houseType");
            writer.Value((int)HouseBuildingTypes.Hotel);
            writer.Name("price");
            writer.Value(Price);
            writer.Name("interiorName");
            writer.Value(HotelRooms.First()?.Interior?.Name);
            writer.Name("position");
            Adapters.PositionAdatper.ToMValue(GetPosition(), writer);
            writer.EndObject();
        }
    }
}