using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleCreatorService
    {
        VehicleModel CreateVehicle(string vehicleModel, Position position, float heading, short dimension, int ownerId);
        Task SaveVehicleToDatabaseAsync(VehicleModel vehicle);
    }
}
