using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IVehiclesManager : IManager<VehicleModel>
    {
        bool TryGetVehicleModel(int vehicleId, out VehicleModel vehicleModel);
        bool TryGetVehicleModel(IMyVehicle vehicle, out VehicleModel vehicleModel);
        bool TryGetVehicleModel(IVehicle vehicle, out VehicleModel vehicleModel);
        VehicleModel GetVehicleModel(ushort vehicleId);
        bool RemoveVehicle(int vehicleId);
        bool RemoveVehicle(VehicleModel vehicle);
        Task<bool> RemoveVehicleFromWorldAsync(VehicleModel vehicle);
        bool IsCharacterOwnerOfVehicle(Character character, VehicleModel vehicle);
        bool HasVehiclePermission(Character character, VehicleModel vehicle);
        List<VehicleModel> GetAllPlayerVehicles(Character character);
        Task<VehicleModel> CreateVehicleAsync(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType);
        VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType);
    }
}
