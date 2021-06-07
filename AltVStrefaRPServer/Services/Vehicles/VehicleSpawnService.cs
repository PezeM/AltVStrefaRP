using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models.Vehicles;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VehicleModel = AltVStrefaRPServer.Models.Vehicles.VehicleModel;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleSpawnService : IVehicleSpawnService
    {
        private IVehicleDatabaseService _vehicleDatabaseService;
        private readonly ILogger<VehicleSpawnService> _logger;

        public VehicleSpawnService(IVehicleDatabaseService vehicelDatabaseService, ILogger<VehicleSpawnService> logger)
        {
            _vehicleDatabaseService = vehicelDatabaseService;
            _logger = logger;
        }

        public async Task SpawnVehicleAsync(VehicleModel vehicleModel)
        {
            if (!CanSpawnVehicle(vehicleModel)) return;

            try
            {
                vehicleModel.VehicleHandle = await AltAsync.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z),
                    new Rotation(vehicleModel.Roll, vehicleModel.Pitch, vehicleModel.Yaw)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error spawning vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
                throw;
            }

            SetVehicleData(vehicleModel);
            _logger.LogDebug("Spawned vehicle VID({vehicleId})", vehicleModel.Id);
        }

        public void SpawnVehicle(VehicleModel vehicleModel)
        {
            if (!CanSpawnVehicle(vehicleModel)) return;

            try
            {
                vehicleModel.VehicleHandle = Alt.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z),
                    new Rotation(vehicleModel.Roll, vehicleModel.Pitch, vehicleModel.Yaw));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error spawning vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
                throw;
            }

            SetVehicleData(vehicleModel);
            _logger.LogDebug("Spawned vehicle VID({vehicleId})", vehicleModel.Id);
        }

        /// <summary>
        /// Despawns vehicle from game and saves its to database
        /// </summary>
        /// <param name="vehicleModel"></param>
        /// <returns></returns>
        public void DespawnVehicle(VehicleModel vehicleModel)
        {
            if (!CanDespawnVehicle(vehicleModel)) return;
            SaveVehicleData(vehicleModel);

            _vehicleDatabaseService.SaveVehicle(vehicleModel);
            Alt.Server.RemoveVehicle(vehicleModel.VehicleHandle);
            _logger.LogDebug("Despawned vehicle VID({vehicleId})", vehicleModel.Id);
        }

        /// <summary>
        /// Despawns vehicle from game and saves its to database
        /// </summary>
        /// <param name="vehicleModel"></param>
        /// <returns></returns>
        public async Task DespawnVehicleAsync(VehicleModel vehicleModel)
        {
            if (!CanDespawnVehicle(vehicleModel)) return;
            SaveVehicleData(vehicleModel);

            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel).ConfigureAwait(false);
            Alt.Server.RemoveVehicle(vehicleModel.VehicleHandle);
            _logger.LogDebug("Despawned vehicle VID({vehicleId})", vehicleModel.Id);
        }

        private bool CanDespawnVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return false;
            if (!vehicleModel.IsSpawned) return false;
            return true;
        }

        private bool CanSpawnVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel == null || vehicleModel.IsSpawned) return false;
            return vehicleModel.VehicleHandle == null;
        }

        private void SaveVehicleData(VehicleModel vehicleModel)
        {
            vehicleModel.IsSpawned = false;
            vehicleModel.X = vehicleModel.VehicleHandle.Position.X;
            vehicleModel.Y = vehicleModel.VehicleHandle.Position.Y;
            vehicleModel.Z = vehicleModel.VehicleHandle.Position.Z;
            vehicleModel.Dimension = vehicleModel.VehicleHandle.Dimension;
            vehicleModel.Pitch = vehicleModel.VehicleHandle.Rotation.Pitch;
            vehicleModel.Yaw = vehicleModel.VehicleHandle.Rotation.Yaw;
            vehicleModel.Roll = vehicleModel.VehicleHandle.Rotation.Roll;
        }

        private void SetVehicleData(VehicleModel vehicleModel)
        {
            if (vehicleModel.VehicleHandle is IMyVehicle myVehicle)
            {
                myVehicle.DatabaseId = vehicleModel.Id;
            }

            //vehicleModel.VehicleHandle.ManualEngineControl = true; // Set on client
            vehicleModel.VehicleHandle.Dimension = vehicleModel.Dimension;
            vehicleModel.IsLocked = false;
            vehicleModel.VehicleHandle.LockState = VehicleLockState.Unlocked;
            vehicleModel.VehicleHandle.SetData(MetaData.VEHICLE_ID, vehicleModel.Id);
            vehicleModel.VehicleHandle.SetSyncedMetaData(MetaData.SYNCED_VEHICLE_ID, vehicleModel.Id);
            vehicleModel.VehicleHandle.SetSyncedMetaData(MetaData.SYNCED_VEHICLE_FUEL, vehicleModel.Fuel);
            vehicleModel.VehicleHandle.SetSyncedMetaData(MetaData.SYNCED_VEHICLE_OIL, vehicleModel.Oil);
            //vehicleModel.VehicleHandle.SetSyncedMetaData(MetaData.SYNCED_VEHICLE_MILEAGE, vehicleModel.Mileage);
            vehicleModel.VehicleHandle.NumberplateText = vehicleModel.PlateText;
            vehicleModel.VehicleHandle.NumberplateIndex = vehicleModel.PlateNumber;
            vehicleModel.IsSpawned = true;
        }
    }
}
