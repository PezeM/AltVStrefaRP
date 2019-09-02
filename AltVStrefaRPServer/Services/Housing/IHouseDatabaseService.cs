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
        Task AddNewHouseAsync(House house);
        Task UpdateHouseAsync(HouseBuilding oldHouse);
        Task UpdateHouseAsync(House house);
        Task UpdateHotelAsync(Hotel hotel);
        Task RemoveHouseAsync(HouseBuilding oldHouse);
    }
}