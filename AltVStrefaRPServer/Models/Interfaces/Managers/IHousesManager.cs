using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<HouseBuilding>
    {
        bool TryGetHouse(int houseId, out HouseBuilding oldHouse);
        bool CheckIfHouseExists(int houseId);
        HouseBuilding GetHouse(int houseId);
        HotelRoom GetHotelRoom(int hotelRoom);
        Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId);
    }
}