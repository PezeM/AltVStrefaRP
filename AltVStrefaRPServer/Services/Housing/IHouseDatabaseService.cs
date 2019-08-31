using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IHouseDatabaseService
    {
        IEnumerable<HouseBuilding> GetAllHouses();
        Task<HouseBuilding> GetHouseAsync(int houseId);
        HouseBuilding GetHouse(int houseId);
        Task AddNewHouseAsync(HouseBuilding oldHouse);
        Task UpdateHouseAsync(HouseBuilding oldHouse);
        Task RemoveHouseAsync(HouseBuilding oldHouse);
    }
}