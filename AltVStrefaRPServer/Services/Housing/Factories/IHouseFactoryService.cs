using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IHouseFactoryService
    {
        OldHouse CreateNewHouse(Position position, int price);
    }
}