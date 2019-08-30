using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IHouseDatabaseService
    {
        IEnumerable<OldHouse> GetAllHouses();
        Task<OldHouse> GetHouseAsync(int houseId);
        OldHouse GetHouse(int houseId);
        Task AddNewHouseAsync(OldHouse oldHouse);
        Task UpdateHouseAsync(OldHouse oldHouse);
        Task RemoveHouseAsync(OldHouse oldHouse);
    }
}