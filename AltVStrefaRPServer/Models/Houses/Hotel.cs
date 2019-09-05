using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Houses.Interfaces;
using AltVStrefaRPServer.Modules.Core;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class Hotel : IHotel
    {
        public int Id { get; private set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int Price { get; set; }
        public int MaximumNumberOfHotelRooms { get; set; }

        public ICollection<HotelRoom> HotelRooms { get; set; }
        
        public IStrefaColshape Colshape { get; private set; }
        public IMarker Marker { get; private set; }

        public void InitializeHouseBuilding()
        {
            Colshape = (IStrefaColshape)Alt.CreateColShapeCylinder(GetPosition(), 1f, 1.5f);
            Colshape.HouseId = Id;

            Marker = MarkerManager.Instance.AddMarker(21, GetPosition(), Color.FromArgb(255, 30, 40, 100),
                new Position(0.8f, 0.8f, 1f), 10, 0);
        }

        
        public bool TryGetHotelRoom(int roomNumber, out HotelRoom hotelRoom)
        {
            hotelRoom = HotelRooms.FirstOrDefault(h => h.HotelRoomNumber == roomNumber);
            return hotelRoom != null;
        }

        public bool AddNewHotelRoom(HotelRoom hotelRoom)
        {
            if (HotelRooms.Count >= MaximumNumberOfHotelRooms) return false;
            HotelRooms.Add(hotelRoom);
            // Set hotel room number
            return true;
        }

        public Position GetPosition()
        {
            return new Position(X, Y, Z);
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("houseType");
            writer.Value((int)HouseBuildingTypes.Hotel);
            writer.Name("price");
            writer.Value(Price);
            //writer.Name("interiorName");
            //writer.Value(HotelRooms.First()?.Interior?.Name);
            writer.Name("position");
            Adapters.PositionAdatper.ToMValue(GetPosition(), writer);
            writer.EndObject();
        }
    }
}