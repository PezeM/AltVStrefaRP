using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<House>
    {
        bool TryGetHouse(int houseId, out House house);
        bool CheckIfHouseExists(int houseId);
        House GetHouse(int houseId);
        Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId);
    }
}