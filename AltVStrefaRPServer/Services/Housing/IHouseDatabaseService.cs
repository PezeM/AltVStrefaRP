using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IHouseDatabaseService
    {
        IEnumerable<HouseBuilding> GetAllHouseBuildings();
        IEnumerable<Hotel> GetAllHotels();
        IEnumerable<House> GetAllHouses();
        Task<HouseBuilding> GetHouseBuildingAsync(int houseId);
        HouseBuilding GetHouseBuilding(int houseId);
        House GetHouse(int houseId);
        Task<House> GetHouseAsync(int houseId);
        Task AddNewHouseAsync(HouseBuilding oldHouse);
        Task UpdateHouseAsync(HouseBuilding oldHouse);
        Task RemoveHouseAsync(HouseBuilding oldHouse);
    }
}