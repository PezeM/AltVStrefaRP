using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using VehicleModel = AltVStrefaRPServer.Models.VehicleModel;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleCreatorService : IVehicleCreatorService
    {
        /// <summary>
        /// Creates <see cref="Models.VehicleModel"/> with default values
        /// </summary>
        /// <param name="vehicleModel">todo: describe vehicleModel parameter on CreateVehicle</param>
        /// <param name="position">todo: describe position parameter on CreateVehicle</param>
        /// <param name="heading">todo: describe heading parameter on CreateVehicle</param>
        /// <param name="dimension">todo: describe dimension parameter on CreateVehicle</param>
        /// <param name="ownerId">todo: describe ownerId parameter on CreateVehicle</param>
        /// <returns></returns>
        public VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType)
        {
            return new VehicleModel
            {
                Owner = ownerId,
                Model = vehicleModel,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Dimension = dimension,
                Yaw = rotation.Yaw,
                Pitch = rotation.Pitch,
                Roll = rotation.Roll,
                OwnerType = ownerType,
                PlateNumber = 0,
                PlateText = "", // Change it to unique plate text
                // Temporary values
                MaxFuel = 50.0f,
                Fuel = 50.0f,
                MaxOil = 10.0f,
                Oil = 5.0f,
                Mileage = 0.0f,
                IsBlocked = false,
                IsJobVehicle = false,
                IsLocked = false,
                IsSpawned = false
            };
        }
    }
}
