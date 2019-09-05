using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Houses.Interfaces;
using AltVStrefaRPServer.Models.Houses.Responses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Housing;
using AltVStrefaRPServer.Services.Housing.Factories;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HousesManager : IHousesManager
    {
        private readonly Dictionary<int, IHouseBuilding> _houseBuildings;
        private readonly IHouseDatabaseService _houseDatabaseService;
        private readonly ILogger<HousesManager> _logger;
        private readonly IInteriorsManager _interiorsManager;
        private readonly IHouseFactoryService _houseFactoryService;

        public HousesManager(IHouseDatabaseService houseDatabaseService, IHouseFactoryService houseFactoryService, IInteriorsManager interiorsManager, 
            ILogger<HousesManager> logger)
        {
            _houseBuildings = new Dictionary<int, IHouseBuilding>();
            _houseDatabaseService = houseDatabaseService;
            _houseFactoryService = houseFactoryService;
            _interiorsManager = interiorsManager;
            _logger = logger;

            InitializeHouses();
        }

        public IHouseBuilding GetLatestHouseBuilding()
        {
            return _houseBuildings.Values.Last();
        }

        public bool TryGetHouseBuilding(int houseId, out IHouseBuilding oldHouse) => _houseBuildings.TryGetValue(houseId, out oldHouse);

        public bool CheckIfHouseExists(int houseId) => _houseBuildings.ContainsKey(houseId);

        public IHouseBuilding GetHouse(int houseId) => CheckIfHouseExists(houseId) ? _houseBuildings[houseId] : null;

        public IEnumerable<THouseBuilding> GetHouseBuildings<THouseBuilding>() where THouseBuilding : IHouseBuilding
        {
            return _houseBuildings.Values.OfType<THouseBuilding>();
        }

        public HotelRoom GetHotelRoom(int hotelRoom)
        {
            foreach (var housesBuilding in _houseBuildings)
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

            var newHouse = _houseFactoryService.CreateNewHouse(position, price, interior);
            await _houseDatabaseService.UpdateHouseAsync(newHouse);

            newHouse.InitializeHouseBuilding();
            _houseBuildings.Add(newHouse.Id, newHouse);

            _logger.LogInformation("Created new house ID({houseId}) at position {position} with price {housePrice} and interior {interiorName}",
                newHouse.Id, position, price, interior.Name);
            return AddNewHouseResponse.HouseCreated;
        }

        public async Task<AddNewHouseResponse> AddNewHotelAsync(Position position, int pricePerRoom, int rooms, int interiorId)
        {
            if (interiorId <= 0) return AddNewHouseResponse.WrongInteriorId;
            if (!_interiorsManager.TryGetInterior(interiorId, out var interior))
                return AddNewHouseResponse.InteriorNotFound;

            var newHotel = _houseFactoryService.CreateNewHotel(new Position(position.X, position.Y, position.Z - 1), pricePerRoom, rooms);
            for (var i = 0; i < rooms; i++)
            {
                var hotelRoom = _houseFactoryService.CreateNewHotelRoom(i + 1);
                interior.HotelRooms.Add(hotelRoom);
            }

            await _houseDatabaseService.UpdateHotelAsync(newHotel); // Don't know if it will work
            newHotel.InitializeHouseBuilding();
            _houseBuildings.Add(newHotel.Id, newHotel);
            
            _logger.LogInformation("Created new hotel ID({houseId}) at position {position} with {hotelRooms} rooms, " +
                                   "price per room {housePrice} and interior {interiorName}", 
                newHotel.Id, position, rooms, pricePerRoom, interior.Name);
            return AddNewHouseResponse.HouseCreated;
        }
        
        private void InitializeHouses()
        {
            var startTime = Time.GetTimestampMs();
            LoadHouses();
            LoadHotels();
            _logger.LogInformation("Loaded {housesCount} house buildings from database in {elapsedTime}ms", 
                _houseBuildings.Count, Time.GetElapsedTime(startTime));
        }

        private void LoadHouses()
        {
            foreach (var house in _houseDatabaseService.GetAllHouses())
            {
                _houseBuildings.Add(house.Id, house);
                house.InitializeHouseBuilding();
            }
        }

        private void LoadHotels()
        {
            foreach (var house in _houseDatabaseService.GetAllHotels())
            {
                _houseBuildings.Add(house.Id, house);
                house.InitializeHouseBuilding();
            }
        }
    }
}