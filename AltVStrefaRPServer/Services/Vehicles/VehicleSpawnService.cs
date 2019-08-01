using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using System;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using Microsoft.Extensions.Logging;
using VehicleModel = AltVStrefaRPServer.Models.VehicleModel;

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
            if(!CanSpawnVehicle(vehicleModel)) return;

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
            _logger.LogDebug("Spawned vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
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
                _logger.LogError(e, "Error spawning vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
                throw;
            }

            SetVehicleData(vehicleModel);
            _logger.LogDebug("Spawned vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
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
            _logger.LogDebug("Despawned vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
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
            _logger.LogDebug("Despawned vehicle {@vehicle} VID({vehicleId})", vehicleModel, vehicleModel.Id);
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

            vehicleModel.VehicleHandle.ManualEngineControl = true;
            vehicleModel.VehicleHandle.Dimension = vehicleModel.Dimension;
            vehicleModel.IsLocked = false;
            vehicleModel.VehicleHandle.LockState = VehicleLockState.Unlocked;
            vehicleModel.VehicleHandle.SetData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.SetSyncedMetaData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.SetSyncedMetaData("fuel", vehicleModel.Fuel);
            vehicleModel.VehicleHandle.SetSyncedMetaData("oil", vehicleModel.Oil);
            vehicleModel.VehicleHandle.SetSyncedMetaData("mileage", vehicleModel.Mileage);
            vehicleModel.VehicleHandle.NumberplateText = vehicleModel.PlateText;
            vehicleModel.VehicleHandle.NumberplateIndex = vehicleModel.PlateNumber;
            vehicleModel.IsSpawned = true;
        }
    }
}
