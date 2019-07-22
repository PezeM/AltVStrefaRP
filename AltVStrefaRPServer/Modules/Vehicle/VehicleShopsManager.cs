using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Services.Vehicles.VehicleShops;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsManager
    {
        private readonly IVehicleShopDatabaseService _vehicleShopDatabaseService;
        private readonly IVehicleShopsFactory _vehicleShopsFactory;

        public List<VehicleShop> VehicleShops { get; private set; }

        public VehicleShopsManager(IVehicleShopDatabaseService vehicleShopDatabaseService, IVehicleShopsFactory vehicleShopsFactory)
        {
            VehicleShops = new List<VehicleShop>();
            _vehicleShopDatabaseService = vehicleShopDatabaseService;
            _vehicleShopsFactory = vehicleShopsFactory;

            LoadVehicleShops();
            CreateDefaultVehicleShops();
        }

        private void LoadVehicleShops()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicleShop in _vehicleShopDatabaseService.GetAllVehicleShops())
            {
                VehicleShops.Add(vehicleShop);
            }
            Alt.Log($"Loaded {VehicleShops.Count} vehicle shops in {Time.GetTimestampMs() - startTime}ms.");
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
            Alt.Server.LogWarning("There were no vehicle shops on the server. Creating new default vehicle shops");
            VehicleShops.AddRange(_vehicleShopsFactory.CreateDefaultVehicleShops(_vehicleShopDatabaseService));
            Alt.Log($"Created {VehicleShops.Count} new vehicle shops.");
        }
    }
}
