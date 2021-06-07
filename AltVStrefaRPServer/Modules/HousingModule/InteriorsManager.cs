using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Houses.Responses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Housing;
using AltVStrefaRPServer.Services.Housing.Factories;
using Microsoft.Extensions.Logging;
using AddNewInteriorResponse = AltVStrefaRPServer.Models.Houses.Responses.AddNewInteriorResponse;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class InteriorsManager : IInteriorsManager
    {
        private readonly Dictionary<int, Interior> _interiors;
        private readonly ILogger<InteriorsManager> _logger;
        private readonly IInteriorDatabaseService _interiorDatabaseService;
        private readonly IInteriorsFactoryService _interiorsFactoryService;

        public InteriorsManager(IInteriorDatabaseService interiorDatabaseService, IInteriorsFactoryService interiorsFactoryService, ILogger<InteriorsManager> logger)
        {
            _interiors = new Dictionary<int, Interior>();
            _interiorDatabaseService = interiorDatabaseService;
            _interiorsFactoryService = interiorsFactoryService;
            _logger = logger;

            InitializeInteriors();
            CreateDefaultInteriors();
        }
        
        public bool TryGetInterior(int interiorId, out Interior interior) => _interiors.TryGetValue(interiorId, out interior);

        public Interior GetInterior(int interiorId) =>
            _interiors.ContainsKey(interiorId) ? _interiors[interiorId] : null;

        public async Task<AddNewInteriorResponse> AddNewInteriorAsync(Position exitPosition, Position enterPosition, string name)
        {
            if (name.IsNullOrEmpty()) return AddNewInteriorResponse.WrongInteriorName;
            // Do some extra regex for interior name
            await _interiorsFactoryService.CreateNewInteriorAsync(exitPosition, enterPosition, name);
            return AddNewInteriorResponse.InteriorCreated;
        }
        
        private void InitializeInteriors()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var interior in _interiorDatabaseService.GetAllInteriors().ToList())
            {
                interior.InitializeInterior();
                _interiors.TryAdd(interior.Id, interior);
            }
            _logger.LogInformation("Loaded {interiorsCount} interiors from database in {elapsedTime} ms", _interiors.Count, Time.GetElapsedTime(startTime));
        }
        
        private void CreateDefaultInteriors()
        {
            if (_interiors.Count > 0) return;
            
            try
            {
                _logger.LogInformation("Not found any interiors. Creating new default interiors");
                var newInteriors = _interiorsFactoryService.CreateDefaultInteriors().ToList();
                _interiorDatabaseService.AddNewInteriors(newInteriors);
                foreach (var interior in newInteriors)
                {
                    interior.InitializeInterior();
                    _interiors.TryAdd(interior.Id, interior);
                }
                _logger.LogInformation("Created {interiorsCount} default interiors", _interiors.Count);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error in loading new default interiors");
                throw;
            }
        }
    }
}