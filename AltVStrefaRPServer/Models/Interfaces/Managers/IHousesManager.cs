using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Houses.Interfaces;
using AltVStrefaRPServer.Models.Houses.Responses;
using AddNewHouseResponse = AltVStrefaRPServer.Models.Houses.Responses.AddNewHouseResponse;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IHousesManager : IManager<IHouseBuilding>
    {
        IHouseBuilding GetLatestHouseBuilding();
        bool TryGetHouseBuilding(int houseId, out IHouseBuilding oldHouse);
        bool CheckIfHouseExists(int houseId);
        IHouseBuilding GetHouse(int houseId);
        IEnumerable<THouseBuilding> GetHouseBuildings<THouseBuilding>() where THouseBuilding : IHouseBuilding;
        HotelRoom GetHotelRoom(int hotelRoom);
        Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId);
        Task<AddNewHouseResponse> AddNewHotelAsync(Position position, int pricePerRoom, int rooms, int interiorId);
    }
}