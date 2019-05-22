using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
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

        public async Task SpawnVehicleAsync(VehicleModel vehicleModel)
        {
            if(!CanSpawnVehicle(vehicleModel)) return;

            try
            {
                vehicleModel.VehicleHandle = await AltAsync.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z), 
                    new Rotation(vehicleModel.Roll, vehicleModel.Pitch, vehicleModel.Yaw)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Alt.Log($"Error creating vehicle with model {vehicleModel.Model} ID({vehicleModel.Id}) ex: {e}");
                throw;
            }

            SetVehicleData(vehicleModel);
            Alt.Log($"Spawned vehicle UID({vehicleModel.Id}) ID({vehicleModel.VehicleHandle.Id})");
        }

        public void SpawnVehicle(VehicleModel vehicleModel)
        {
            if(!CanSpawnVehicle(vehicleModel)) return;

            try
            {
                vehicleModel.VehicleHandle = Alt.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z), 
                    new Rotation(vehicleModel.Roll, vehicleModel.Pitch, vehicleModel.Yaw));
            }
            catch (Exception e)
            {
                Alt.Log($"Error creating vehicle with model {vehicleModel.Model} ID({vehicleModel.Id}) ex: {e}");
                throw;
            }

            SetVehicleData(vehicleModel);
            Alt.Log($"Spawned vehicle UID({vehicleModel.Id}) ID({vehicleModel.VehicleHandle.Id})");
        }

        private bool CanSpawnVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return false;
            return !vehicleModel.IsSpawned && vehicleModel.VehicleHandle == null;
        }

        private void SetVehicleData(VehicleModel vehicleModel)
        {
            //vehicleModel.VehicleHandle.ManualEngineControl = true;
            vehicleModel.VehicleHandle.Dimension = vehicleModel.Dimension;
            vehicleModel.IsLocked = false;
            vehicleModel.VehicleHandle.LockState = VehicleLockState.Unlocked;
            vehicleModel.VehicleHandle.SetData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.SetSyncedMetaData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.NumberplateText = vehicleModel.PlateText;
            vehicleModel.VehicleHandle.NumberplateIndex = vehicleModel.PlateNumber;
            vehicleModel.IsSpawned = true;
        }
    }
}
