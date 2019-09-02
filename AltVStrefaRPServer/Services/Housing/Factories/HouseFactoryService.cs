using System.Collections.Generic;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class HouseFactoryService : IHouseFactoryService
    {
        public Flat CreateNewFlat()
        {
            var flat = new Flat();

            flat.CreateLockPattern();
            return flat;
        }

        public HotelRoom CreateNewHotelRoom(int roomNumber)
        {
            var hotelRoom = new HotelRoom
            {
                HotelRoomNumber = 1
            };
            hotelRoom.CreateLockPattern();

            return hotelRoom;
        }
        
        public House CreateNewHouse(Position position, int price)
        {
            var newHouse = new House
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Price = price,
                Flat = CreateNewFlat(),
            };

            return newHouse;
        }

        public Hotel CreateNewHotel(Position position, int price, int maxRooms)
        {
            var newHotel = new Hotel
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Price = price,
                MaximumNumberOfRooms = maxRooms,
                HotelRooms = new List<HotelRoom>()
            };
            
            return newHotel;
        }
    }
}