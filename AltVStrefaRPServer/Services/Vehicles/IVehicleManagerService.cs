using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleManagerService
    {
        Task<List<VehicleModel>> LoadVehiclesFromDatabaseAsync();
        Task RemoveVehicleAsync(VehicleModel vehicle);
    }
}
