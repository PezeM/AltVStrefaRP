using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class HouseFactoryService : IHouseFactoryService
    {
        public Flat CreateNewFlat()
        {
            var flat = new Flat
            {
                IsLocked = true,
            };
            
            flat.CreateLockPattern();
            return new Flat();
        }

        public HotelRoom CreateNewHouseRoom(int roomNumber)
        {
            return new HotelRoom
            {
//                Interior = interior,
                IsLocked = true,
                HotelRoomNumber = roomNumber
            };
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
                MaximumNumberOfRooms = maxRooms
            };
            
            return newHotel;
        }
    }
}