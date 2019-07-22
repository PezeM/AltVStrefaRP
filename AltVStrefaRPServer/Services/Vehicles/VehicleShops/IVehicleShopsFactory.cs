using System.Collections.Generic;
using AltVStrefaRPServer.Modules.Vehicle;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
{
    public interface IVehicleShopsFactory
    {
        IEnumerable<VehicleShop> CreateDefaultVehicleShops(IVehicleShopDatabaseService vehicleShopDatabaseService);
    }
}
