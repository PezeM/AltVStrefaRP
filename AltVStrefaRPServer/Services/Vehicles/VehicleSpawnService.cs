using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using System;
using System.Threading.Tasks;
using VehicleModel = AltVStrefaRPServer.Models.VehicleModel;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleSpawnService : IVehicleSpawnService
    {
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
