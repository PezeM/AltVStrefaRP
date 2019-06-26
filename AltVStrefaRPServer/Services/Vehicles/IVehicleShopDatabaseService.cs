using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Modules.Vehicle;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleShopDatabaseService
    {
        IEnumerable<VehicleShop> GetAllVehicleShops();
        Task SaveVehicleShop(VehicleShop shop);
    }
}
