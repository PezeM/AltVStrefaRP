using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Housing
{
    public class HouseDatabaseService : IHouseDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public HouseDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<HouseBuilding> GetAllHouseBuildings()
        {
            using (var context = _factory.Invoke())
            {
                return context.HouseBuildings.ToList();
            }
        }

        public IEnumerable<Hotel> GetAllHotels()
        {
            using (var context = _factory.Invoke())
            {
                return context.Hotels
                    .Include(h => h.HotelRooms)
                        .ThenInclude(hr => hr.Interior)
                    .Include(h => h.HotelRooms)
                        .ThenInclude(hr => hr.HouseBuilding)
                    .ToList();
            }
        }

        public IEnumerable<House> GetAllHouses()
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses
                    .Include(h => h.Flat)
                        .ThenInclude(f => f.Interior)
                    .Include(h => h.Flat)
                        .ThenInclude(f => f.HouseBuilding)
                    .ToList();
            }
        }

        public async Task<HouseBuilding> GetHouseBuildingAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.HouseBuildings.FindAsync(houseId);
            }
        }

        public HouseBuilding GetHouseBuilding(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.HouseBuildings.Find(houseId);
            }
        }

        public House GetHouse(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses
                    .Include(h => h.Flat)
                        .ThenInclude(f => f.Interior)
                    .FirstOrDefault(q => q.Id == houseId);
            }
        }

        public async Task<House> GetHouseAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Houses
                    .Include(h => h.Flat)
                    .ThenInclude(f => f.Interior)
                    .FirstOrDefaultAsync(q => q.Id == houseId);
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

        public async Task AddNewHouseAsync(House house)
        {
            using (var context = _factory.Invoke())
            {
                context.Houses.Add(house);
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

        public async Task UpdateHouseAsync(House house)
        {
            using (var context = _factory.Invoke())
            {
                context.Houses.Update(house);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            using (var context = _factory.Invoke())
            {
                context.Hotels.Update(hotel);
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