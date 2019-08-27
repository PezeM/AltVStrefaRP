using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<House>
    {
        bool TryGetHouse(int houseId, out House house);
        bool CheckIfHouseExists(int houseId);
        House GetHouse(int houseId);
    }
}