using System.Collections.Generic;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class HouseFactoryService : IHouseFactoryService
    {
        public House CreateNewHouse(Position position, int price)
        {
            var newHouse = new House
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Price = price,
            };
            newHouse.CreateLockPattern();
            return newHouse;
        }

        public House CreateNewHouse(Position position, int price, Interior interior)
        {
            var newHouse = CreateNewHouse(position, price);
            newHouse.Interior = interior;
            return newHouse;
        }

        public HotelRoom CreateNewHotelRoom(int roomNumber)
        {
            var hotelRoom = new HotelRoom
            {
                HotelRoomNumber = roomNumber
            };
            hotelRoom.CreateLockPattern();

            return hotelRoom;
        }

        public Hotel CreateNewHotel(Position position, int price, int maxRooms)
        {
            var newHotel = new Hotel
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Price = price,
                MaximumNumberOfHotelRooms = maxRooms,
                HotelRooms = new List<HotelRoom>()
            };
            newHotel.AddNewHotelRoom(CreateNewHotelRoom(1));

            return newHotel;
        }
    }
}