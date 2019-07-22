using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Modules.Vehicle;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
{
    public interface IVehicleShopDatabaseService
    {
        IEnumerable<VehicleShop> GetAllVehicleShops();
        Task SaveVehicleShopAsync(VehicleShop shop);
        void AddNewVehicleShop(VehicleShop shop);
    }
}
