using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Housing;
using AltVStrefaRPServer.Services.Housing.Factories;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HousesManager : IHousesManager
    {
        private readonly Dictionary<int, HouseBuilding> _housesBuildings;
        private readonly IHouseDatabaseService _houseDatabaseService;
        private readonly ILogger<HousesManager> _logger;
        private readonly IInteriorsManager _interiorsManager;
        private readonly IHouseFactoryService _houseFactoryService;

        public HousesManager(IHouseDatabaseService houseDatabaseService, IHouseFactoryService houseFactoryService, IInteriorsManager interiorsManager, ILogger<HousesManager> logger)
        {
            _housesBuildings = new Dictionary<int, HouseBuilding>();
            _houseDatabaseService = houseDatabaseService;
            _houseFactoryService = houseFactoryService;
            _interiorsManager = interiorsManager;
            _logger = logger;

            InitializeHouses();
        }

        public bool TryGetHouseBuilding(int houseId, out HouseBuilding oldHouse) => _housesBuildings.TryGetValue(houseId, out oldHouse);

        public bool CheckIfHouseExists(int houseId) => _housesBuildings.ContainsKey(houseId);

        public HouseBuilding GetHouse(int houseId) => CheckIfHouseExists(houseId) ? _housesBuildings[houseId] : null;

        public HotelRoom GetHotelRoom(int hotelRoom)
        {
            foreach (var housesBuilding in _housesBuildings)
            {
                if (housesBuilding.Value is Hotel hotel)
                {
                    return hotel.HotelRooms.FirstOrDefault(h => h.HotelRoomNumber == hotelRoom);
                }
            }

            return null;
        }
        
        public async Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId)
        {
            if (interiorId <= 0) return AddNewHouseResponse.WrongInteriorId;
            if (!_interiorsManager.TryGetInterior(interiorId, out var interior))
                return AddNewHouseResponse.InteriorNotFound;

            var newHouse = _houseFactoryService.CreateNewHouse(position, price);
            interior.Flats.Add(newHouse.Flat);
            newHouse.Flat.Interior = interior;
            await _houseDatabaseService.UpdateHouseAsync(newHouse); // Don't know if it will work like that
            newHouse.InitializeHouse();
            _housesBuildings.Add(newHouse.Id, newHouse);
            
            _logger.LogInformation("Created new house ID({houseId}) at position {position} with price {housePrice} and interior {interiorName}", 
                newHouse.Id, position, price, interior.Name);
            return AddNewHouseResponse.HouseCreated;
        }

        public async Task<AddNewHouseResponse> AddNewHotelAsync(Position position, int pricePerRoom, int rooms, int interiorId)
        {
            if (interiorId <= 0) return AddNewHouseResponse.WrongInteriorId;
            if (!_interiorsManager.TryGetInterior(interiorId, out var interior))
                return AddNewHouseResponse.InteriorNotFound;

            var newHotel = _houseFactoryService.CreateNewHotel(position, pricePerRoom, rooms);
            for (var i = 0; i < rooms; i++)
            {
                var hotelRoom = _houseFactoryService.CreateNewHotelRoom(i + 1);
                newHotel.HotelRooms.Add(hotelRoom);
                interior.Flats.Add(hotelRoom);
            }

            await _houseDatabaseService.UpdateHouseAsync(newHotel); // Don't know if it will work
            newHotel.InitializeHouse();
            _housesBuildings.Add(newHotel.Id, newHotel);
            
            _logger.LogInformation("Created new hotel ID({houseId}) at position {position} with {hotelRooms} rooms, price per room {housePrice} and interior {interiorName}", 
                newHotel.Id, position, rooms, pricePerRoom, interior.Name);
            return AddNewHouseResponse.HouseCreated;
        }
        
        private void InitializeHouses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var house in _houseDatabaseService.GetAllHouses())
            {
                _housesBuildings.Add(house.Id, house);
                house.InitializeHouse();
            }
            _logger.LogInformation("Loaded {housesCount} houses from database in {elapsedTime}ms", _housesBuildings.Count, Time.GetElapsedTime(startTime));
        }
    }
}