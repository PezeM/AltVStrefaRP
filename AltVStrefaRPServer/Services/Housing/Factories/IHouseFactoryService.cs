using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IHouseFactoryService
    {
        House CreateNewHouse(Position position, float price);
    }
}