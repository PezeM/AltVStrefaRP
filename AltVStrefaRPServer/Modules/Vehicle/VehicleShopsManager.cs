using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Services.Vehicles;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsManager
    {
        private readonly IVehicleShopDatabaseService _vehicleShopDatabaseService;

        public List<VehicleShop> VehicleShops { get; private set; }

        public VehicleShopsManager(IVehicleShopDatabaseService vehicleShopDatabaseService)
        {
            VehicleShops = new List<VehicleShop>();
            _vehicleShopDatabaseService = vehicleShopDatabaseService;

            LoadVehicleShops();
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
    }
}
