using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IHouseFactoryService
    {
        House CreateNewHouse(Position position, int price);
        House CreateNewHouse(Position position, int price, Interior interior);
        HotelRoom CreateNewHotelRoom(int roomNumber);
        Hotel CreateNewHotel(Position position, int price, int maxRooms);
    }
}