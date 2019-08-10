using System;
using System.Collections.Generic;
using System.IO;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Models.Vehicles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Data
{
    public class VehiclesData
    {
        public Dictionary<string, VehicleData> Data { get; }
        private readonly AppSettings _appSettings;
        private readonly ILogger<VehiclesData> _logger;

        public VehiclesData(AppSettings appSettings, ILogger<VehiclesData> logger)
        {
            Data = new Dictionary<string, VehicleData>();
            _appSettings = appSettings;
            _logger = logger;

            if (!File.Exists(_appSettings.VehiclesDataPath))
            {
                _logger.LogCritical("Path to vehicles data was invalid!");
                throw new Exception("Path to vehicles data was invalid!");
            }

            try
            {
                Data = JsonConvert.DeserializeObject<Dictionary<string, VehicleData>>(File.ReadAllText(_appSettings.VehiclesDataPath));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Error occured while reading vehicles data. Path to file was {_appSettings.VehiclesDataPath}");
                throw;
            }
        }
    }
}
