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

        public IEnumerable<OldHouse> GetAllHouses()
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses.ToList();
            }
        }

        public async Task<OldHouse> GetHouseAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Houses.FindAsync(houseId);
            }
        }

        public OldHouse GetHouse(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses.Find(houseId);
            }
        }

        public async Task AddNewHouseAsync(OldHouse oldHouse)
        {
            using (var context = _factory.Invoke())
            {
                context.Houses.Add(oldHouse);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateHouseAsync(OldHouse oldHouse)
        {
            using (var context = _factory.Invoke())
            { 
                context.Houses.Update(oldHouse);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveHouseAsync(OldHouse oldHouse)
        {
            using (var context = _factory.Invoke())
            { 
                context.Houses.Remove(oldHouse);
                await context.SaveChangesAsync();
            }
        }
    }
}