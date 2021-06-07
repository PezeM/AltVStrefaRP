using AltVStrefaRPServer.Modules.Vehicle;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
{
    public interface IVehicleShopDatabaseService
    {
        IEnumerable<VehicleShop> GetAllVehicleShops();
        Task SaveVehicleShopAsync(VehicleShop shop);
        void AddNewVehicleShop(VehicleShop shop);
    }
}
