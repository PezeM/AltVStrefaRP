using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleCreatorService
    {
        VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType);
    }
}
