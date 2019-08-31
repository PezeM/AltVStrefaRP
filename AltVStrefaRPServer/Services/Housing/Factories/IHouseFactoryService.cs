using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IHouseFactoryService
    {
        Flat CreateNewFlat();
        HotelRoom CreateNewHotelRoom(int roomNumber);
        House CreateNewHouse(Position position, int price);
        Hotel CreateNewHotel(Position position, int price, int maxRooms);
    }
}