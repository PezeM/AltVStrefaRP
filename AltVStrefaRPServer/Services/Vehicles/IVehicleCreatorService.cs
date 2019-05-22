using AltV.Net.Data;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleCreatorService
    {
        VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType);
        Task SpawnVehicleAsync(VehicleModel vehicleModel);
        void SpawnVehicle(VehicleModel vehicleModel);
    }
}
