using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleCreatorService
    {
        VehicleModel CreateVehicle(string vehicleModel, Position position, float heading, short dimension, int ownerId, OwnerType ownerType);
        Task SaveVehicleToDatabaseAsync(VehicleModel vehicle);
        void SaveVehicleToDatabase(VehicleModel vehicle);
    }
}
