using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IHouseDatabaseService
    {
        IEnumerable<Hotel> GetAllHotels();
        IEnumerable<House> GetAllHouses();
        House GetHouse(int houseId);
        Task<House> GetHouseAsync(int houseId);
        Hotel GetHotel(int hotelId);
        Task<Hotel> GetHotelAsync(int hotelId);
        Task AddNewHouseAsync(House house);
        Task UpdateHouseAsync(House house);
        Task UpdateHotelAsync(Hotel hotel);
    }
}