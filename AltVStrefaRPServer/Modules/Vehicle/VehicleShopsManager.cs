using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Services.Vehicles.VehicleShops;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsManager
    {
        private readonly IVehicleShopDatabaseService _vehicleShopDatabaseService;
        private readonly IVehicleShopsFactory _vehicleShopsFactory;
        private readonly ILogger<VehicleShopsManager> _logger;

        public List<VehicleShop> VehicleShops { get; private set; }

        public VehicleShopsManager(IVehicleShopDatabaseService vehicleShopDatabaseService, IVehicleShopsFactory vehicleShopsFactory,
            ILogger<VehicleShopsManager> logger)
        {
            VehicleShops = new List<VehicleShop>();
            _vehicleShopDatabaseService = vehicleShopDatabaseService;
            _vehicleShopsFactory = vehicleShopsFactory;
            _logger = logger;

            LoadVehicleShops();
            CreateDefaultVehicleShops();
        }

        private void LoadVehicleShops()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicleShop in _vehicleShopDatabaseService.GetAllVehicleShops())
            {
                VehicleShops.Add(vehicleShop);
                vehicleShop.CreateBlip();
            }
            _logger.LogInformation("Loaded {vehicleShopsCount} vehicle shops in {elapsedTime}ms", VehicleShops.Count, Time.GetElapsedTime(startTime));
        }

        public VehicleShop GetVehicleShop(int shopId) => VehicleShops.FirstOrDefault(s => s.VehicleShopId == shopId);

        public VehicleShop GetClosestVehicleShop(Position position, int range = 10)
            => VehicleShops.FirstOrDefault(s => s.GetPosition().Distance(position) < range);

        public bool IsNearVehicleShop(Position position, out VehicleShop vehicleShop, int range = 10)
        {
            vehicleShop = VehicleShops.FirstOrDefault(s => s.GetPosition().Distance(position) < range);
            return vehicleShop != null;
        }

        private void CreateDefaultVehicleShops()
        {
            if (VehicleShops.Count > 0) return;
            _logger.LogWarning("There were no vehicle shops on the server. Creating new default vehicle shops");
            VehicleShops.AddRange(_vehicleShopsFactory.CreateDefaultVehicleShops(_vehicleShopDatabaseService));
            _logger.LogInformation("Created {vehicleShopsCount} new vehicle shops", VehicleShops.Count);
        }
    }
}
