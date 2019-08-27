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

        public IEnumerable<House> GetAllHouses()
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses.ToList();
            }
        }

        public async Task<House> GetHouseAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Houses.FindAsync(houseId);
            }
        }

        public House GetHouse(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses.Find(houseId);
            }
        }

        public async Task AddNewHouseAsync(House house)
        {
            using (var context = _factory.Invoke())
            {
                context.Houses.Add(house);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateHouseAsync(House house)
        {
            using (var context = _factory.Invoke())
            { 
                context.Houses.Update(house);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveHouseAsync(House house)
        {
            using (var context = _factory.Invoke())
            { 
                context.Houses.Remove(house);
                await context.SaveChangesAsync();
            }
        }
    }
}