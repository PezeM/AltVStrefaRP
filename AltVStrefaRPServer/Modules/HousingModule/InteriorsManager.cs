using System.Collections.Generic;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Housing;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class InteriorsManager : IInteriorsManager
    {
        private Dictionary<int, Interior> _interiors;
        private readonly ILogger<InteriorsManager> _logger;
        private readonly IInteriorDatabaseService _interiorDatabaseService;

        public InteriorsManager(IInteriorDatabaseService interiorDatabaseService, ILogger<InteriorsManager> logger)
        {
            _interiors = new Dictionary<int, Interior>();
            _interiorDatabaseService = interiorDatabaseService;
            _logger = logger;

            InitializeInteriors();
        }

        public bool TryGetInterior(int interiorId, out Interior interior) => _interiors.TryGetValue(interiorId, out interior);

        public Interior GetInterior(int interiorId) =>
            _interiors.ContainsKey(interiorId) ? _interiors[interiorId] : null;
        
        private void InitializeInteriors()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var interior in _interiorDatabaseService.GetAllInteriors())
            {
                _interiors.TryAdd(interior.Id, interior);
            }
            _logger.LogInformation("Loaded {interiorsCount} interiors from database in {elapsedTime} ms", _interiors.Count, Time.GetElapsedTime(startTime));
        }
    }
}