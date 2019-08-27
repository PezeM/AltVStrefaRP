using System.Collections.Generic;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Housing;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HousesManager : IHousesManager
    {
        private readonly Dictionary<int, House> _houses;
        private readonly IHouseDatabaseService _houseDatabaseService;
        private readonly ILogger<HousesManager> _logger;

        public HousesManager(IHouseDatabaseService houseDatabaseService, ILogger<HousesManager> logger)
        {
            _houses = new Dictionary<int, House>();
            _houseDatabaseService = houseDatabaseService;
            _logger = logger;

            InitializeHouses();
        }

        private void InitializeHouses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var house in _houseDatabaseService.GetAllHouses())
            {
                _houses.Add(house.Id, house);
            }
            _logger.LogInformation("Loaded {housesCount} houses from database in {elapsedTime}", _houses.Count, Time.GetElapsedTime(startTime));
        }
    }
}