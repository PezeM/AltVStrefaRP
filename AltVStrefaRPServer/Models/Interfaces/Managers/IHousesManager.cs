using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<HouseBuilding>
    {
        bool TryGetHouseBuilding(int houseId, out HouseBuilding oldHouse);
        bool CheckIfHouseExists(int houseId);
        HouseBuilding GetHouse(int houseId);
        HotelRoom GetHotelRoom(int hotelRoom);
        Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId);
        Task<AddNewHouseResponse> AddNewHotelAsync(Position position, int pricePerRoom, int rooms, int interiorId);
    }
}