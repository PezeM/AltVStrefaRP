using System.Collections.Generic;
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
        private readonly Dictionary<int, House> _houses;
        private readonly IHouseDatabaseService _houseDatabaseService;
        private readonly ILogger<HousesManager> _logger;
        private readonly IInteriorsManager _interiorsManager;
        private readonly IHouseFactoryService _houseFactoryService;

        public HousesManager(IHouseDatabaseService houseDatabaseService, IHouseFactoryService houseFactoryService, IInteriorsManager interiorsManager, ILogger<HousesManager> logger)
        {
            _houses = new Dictionary<int, House>();
            _houseDatabaseService = houseDatabaseService;
            _houseFactoryService = houseFactoryService;
            _interiorsManager = interiorsManager;
            _logger = logger;

            InitializeHouses();
        }

        public bool TryGetHouse(int houseId, out House house) => _houses.TryGetValue(houseId, out house);

        public bool CheckIfHouseExists(int houseId) => _houses.ContainsKey(houseId);

        public House GetHouse(int houseId) => CheckIfHouseExists(houseId) ? _houses[houseId] : null;

        public async Task<AddNewHouseResponse> AddNewHouseAsync(Position position, int price, int interiorId)
        {
            if (interiorId <= 0) return AddNewHouseResponse.WrongInteriorId;
            if (_interiorsManager.TryGetInterior(interiorId, out var interior))
                return AddNewHouseResponse.InteriorNotFound;

            var newHouse = _houseFactoryService.CreateNewHouse(position, price);
            interior.Houses.Add(newHouse);
            await _houseDatabaseService.AddNewHouseAsync(newHouse); // Don't know if it will work like that
            _houses.Add(newHouse.Id, newHouse);
            
            _logger.LogInformation("Created new house ID({houseId}) at position {position} with price {housePrice} and interior {interiorName}", newHouse.Id, position, price, interior.Name);
            return AddNewHouseResponse.HouseCreated;
        }
        
        private void InitializeHouses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var house in _houseDatabaseService.GetAllHouses())
            {
                _houses.Add(house.Id, house);
                house.InitializeHouse();
            }
            _logger.LogInformation("Loaded {housesCount} houses from database in {elapsedTime}ms", _houses.Count, Time.GetElapsedTime(startTime));
        }
    }
}