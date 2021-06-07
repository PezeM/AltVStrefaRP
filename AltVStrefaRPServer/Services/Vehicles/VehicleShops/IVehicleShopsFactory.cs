using AltVStrefaRPServer.Modules.Vehicle;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
{
    public interface IVehicleShopsFactory
    {
        IEnumerable<VehicleShop> CreateDefaultVehicleShops(IVehicleShopDatabaseService vehicleShopDatabaseService);
    }
}
