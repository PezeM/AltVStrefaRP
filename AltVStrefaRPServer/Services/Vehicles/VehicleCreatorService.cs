using AltV.Net.Data;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleCreatorService : IVehicleCreatorService
    {
        private readonly VehiclesData _vehiclesData;

        public VehicleCreatorService(VehiclesData vehiclesData)
        {
            _vehiclesData = vehiclesData;
        }

        /// <summary>
        /// Creates <see cref="VehicleModel"/> with default values
        /// </summary>
        /// <param name="vehicleModel">todo: describe vehicleModel parameter on CreateVehicle</param>
        /// <param name="position">todo: describe position parameter on CreateVehicle</param>
        /// <param name="rotation">todo: describe heading parameter on CreateVehicle</param>
        /// <param name="dimension">todo: describe dimension parameter on CreateVehicle</param>
        /// <param name="ownerId">todo: describe ownerId parameter on CreateVehicle</param>
        /// <returns></returns>
        public VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, OwnerType ownerType)
        {
            var vehicle = new VehicleModel
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
                PlateText = "BRAK", // Change it to unique plate text
                // Temporary values
                Mileage = 0.0f,
                IsBlocked = false,
                IsJobVehicle = false,
                IsLocked = false,
                IsSpawned = false
            };

            if (_vehiclesData.Data.TryGetValue(vehicleModel, out var vehicleData))
            {
                GenerateValuesFromVehicleData(vehicle, vehicleData);
            }
            else
            {
                GenerateDefaultValues(vehicle);
            }

            return vehicle;
        }

        private void GenerateValuesFromVehicleData(VehicleModel vehicle, VehicleData vehicleData)
        {
            vehicle.Inventory = new VehicleInventoryContainer(vehicleData.InventorySlots);
            vehicle.MaxFuel = vehicleData.MaxFuel;
            vehicle.Fuel = vehicleData.MaxFuel;
            vehicle.MaxOil = vehicleData.MaxOil;
            vehicle.Oil = vehicleData.MaxOil;
        }

        private void GenerateDefaultValues(VehicleModel vehicle)
        {
            vehicle.Inventory = new VehicleInventoryContainer(0);
            vehicle.MaxFuel = 30f;
            vehicle.Fuel = 30f;
            vehicle.MaxOil = 5f;
            vehicle.Oil = 5f;
        }
    }
}
