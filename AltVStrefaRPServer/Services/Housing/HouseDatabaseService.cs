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

        public IEnumerable<Hotel> GetAllHotels()
        {
            using (var context = _factory.Invoke())
            {
                return context.Hotels
                    .Include(h => h.HotelRooms)
                        .ThenInclude(hr => hr.Interior)
                    .Include(h => h.HotelRooms)
                        .ThenInclude(hr => hr.Hotel)
                    .ToList();
            }
        }

        public IEnumerable<House> GetAllHouses()
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses
                    .Include(h => h.Interior)
                    //.Include(h => h.Owner) Maybe will add later
                    .ToList();
            }
        }

        public House GetHouse(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Houses
                    .Include(h => h.Interior)
                    .FirstOrDefault(q => q.Id == houseId);
            }
        }

        public async Task<House> GetHouseAsync(int houseId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Houses
                    .Include(h => h.Interior)
                    .FirstOrDefaultAsync(q => q.Id == houseId);
            }
        }

        public Hotel GetHotel(int hotelId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Hotels
                    .Include(h => h.HotelRooms)
                    .ThenInclude(hr => hr.Interior)
                    .Include(h => h.HotelRooms)
                    .ThenInclude(hr => hr.Hotel)
                    .FirstOrDefault(q => q.Id == hotelId);
            }
        }

        public async Task<Hotel> GetHotelAsync(int hotelId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Hotels
                    .Include(h => h.HotelRooms)
                    .ThenInclude(hr => hr.Interior)
                    .Include(h => h.HotelRooms)
                    .ThenInclude(hr => hr.Hotel)
                    .FirstOrDefaultAsync(q => q.Id == hotelId);
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

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            using (var context = _factory.Invoke())
            {
                context.Hotels.Update(hotel);
                await context.SaveChangesAsync();
            }
        }
    }
}