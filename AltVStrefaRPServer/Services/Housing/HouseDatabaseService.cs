using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public class HouseDatabaseService : IHouseDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public HouseDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<HouseBuilding> GetAllHouses()
        {
            using (var context = _factory.Invoke())
            {
                return context.HouseBuildings.ToList();
            }
        }

        public async Task<HouseBuilding> GetHouseAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.HouseBuildings.FindAsync(houseId);
            }
        }

        public HouseBuilding GetHouse(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.HouseBuildings.Find(houseId);
            }
        }

        public async Task AddNewHouseAsync(HouseBuilding oldHouse)
        {
            using (var context = _factory.Invoke())
            {
                context.HouseBuildings.Add(oldHouse);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateHouseAsync(HouseBuilding oldHouse)
        {
            using (var context = _factory.Invoke())
            { 
                context.HouseBuildings.Update(oldHouse);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveHouseAsync(HouseBuilding oldHouse)
        {
            using (var context = _factory.Invoke())
            { 
                context.HouseBuildings.Remove(oldHouse);
                await context.SaveChangesAsync();
            }
        }
    }
}