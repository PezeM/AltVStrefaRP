using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleManagerService
    {
        Task<List<VehicleModel>> LoadVehiclesFromDatabaseAsync();
        List<VehicleModel> LoadVehiclesFromDatabase();
        Task RemoveVehicleAsync(VehicleModel vehicle);
        void SaveVehicle(VehicleModel vehicle);
        Task SaveVehicleAsync(VehicleModel vehicle);
    }
}
