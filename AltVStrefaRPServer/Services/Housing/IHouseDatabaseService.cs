using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IHouseDatabaseService
    {
        IEnumerable<House> GetAllHouses();
        Task<House> GetHouseAsync(int houseId);
        House GetHouse(int houseId);
        Task AddNewHouseAsync(House house);
        Task UpdateHouseAsync(House house);
        Task RemoveHouseAsync(House house);
    }
}