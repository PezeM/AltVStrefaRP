using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<OldHouse>
    {
        bool TryGetHouse(int houseId, out OldHouse oldHouse);
        bool CheckIfHouseExists(int houseId);
        OldHouse GetHouse(int houseId);
        Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId);
    }
}