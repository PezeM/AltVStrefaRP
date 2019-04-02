using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Services.Vehicles;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleManager
    {
        private Dictionary<int, VehicleModel> Vehicles = new Dictionary<int, VehicleModel>();
        private IVehicleManagerService _vehicleManagerService;
        private IVehicleCreatorService _vehicleCreator;

        public VehicleManager(IVehicleManagerService vehicleManagerService, IVehicleCreatorService vehicleCreatorService)
        {
            _vehicleManagerService = vehicleManagerService;
            _vehicleCreator = vehicleCreatorService;

            LoadVehiclesFromDatabaseAsync();
        }

        /// <summary>
        /// Loads all vehicles from database to memory and sets every vehicle IsSpawned property to false
        /// </summary>
        public async Task LoadVehiclesFromDatabaseAsync()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicle in await _vehicleManagerService.LoadVehiclesFromDatabaseAsync().ConfigureAwait(false))
            {
                vehicle.IsSpawned = false;
                Vehicles.Add(vehicle.Id, vehicle);
            }
            Alt.Log($"Loaded {Vehicles.Count} vehicles from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        /// <summary>
        /// Gets VehicleModel by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>Returns <see cref="VehicleModel"/></returns>
        public VehicleModel GetVehicleModel(int vehicleId) => Vehicles.ContainsKey(vehicleId) ? Vehicles[vehicleId] : null;

        /// <summary>
        /// Gets VehicleModel by vehicleHandle id
        /// </summary>
        /// <param name="vehicleID">Id of vehicle handle</param>
        /// <returns></returns>
        public VehicleModel GetVehicleModel(ushort vehicleID) => Vehicles.Values.FirstOrDefault(v => v.VehicleHandle?.Id == vehicleID);

        /// <summary>
        /// Gets <see cref="VehicleModel"/> by vehicle handle id
        /// </summary>
        /// <param name="vehicle"><see cref="IVehicle"/></param>
        /// <returns></returns>
        public VehicleModel GetVehicleModel(IVehicle vehicle) => Vehicles.Values.FirstOrDefault(v => v.VehicleHandle?.Id == vehicle.Id);

        /// <summary>
        /// Removes <see cref="VehicleModel"/> from vehicle list by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>True if vehicle was removed successfully</returns>
        public bool RemoveVehicle(int vehicleId) => Vehicles.Remove(vehicleId);

        /// <summary>
        /// Removes <see cref="VehicleModel"/> from vehicle list by value
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>True if vehicle was removed successfully</returns>
        public bool RemoveVehicle(VehicleModel vehicle) => Vehicles.Remove(vehicle.Id);

        /// <summary>
        /// Completly removes vehicle. Removes it from the server/vehicle list and database
        /// </summary>
        /// <param name="vehicle">The vehicle to remove</param>
        /// <returns></returns>
        public async Task<bool> RemoveVehicleFromWorldAsync(VehicleModel vehicle)
        {
            if (RemoveVehicle(vehicle))
            {
                try
                {
                    Alt.RemoveVehicle(vehicle.VehicleHandle);
                    await _vehicleManagerService.RemoveVehicleAsync(vehicle).ConfigureAwait(false);
                    return true;
                }
                catch (Exception e)
                {
                    Alt.Log($"Error removing vehicle from world: {e}");
                    throw;
                }
            }
            return false;
        }

        public async Task<VehicleModel> CreateVehicleAsync(string vehicleModel, Position position, float heading, short dimension, int ownerId)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, heading, dimension, ownerId);
            await _vehicleCreator.SaveVehicleToDatabaseAsync(vehicle).ConfigureAwait(false);
            Vehicles.Add(vehicle.Id, vehicle);
            Alt.Log($"Created vehicle {vehicle.Model} UID({vehicle.Id}).");
            return vehicle;
        }

        public void SpawnVehicle(int vehicleId)
        {
            SpawnVehicle(GetVehicleModel(vehicleId));
        }

        public void SpawnVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return;
            if (vehicleModel.IsSpawned) return;
            if (vehicleModel.VehicleHandle != null) return;

            vehicleModel.VehicleHandle = Alt.CreateVehicle(vehicleModel.Model,
                new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z), vehicleModel.Heading);

            vehicleModel.VehicleHandle.EngineOn = false;
            vehicleModel.VehicleHandle.SetData("vehicleId", vehicleModel.Id);
            vehicleModel.IsSpawned = true;

            Alt.Log($"Spawned vehicle UID({vehicleModel.Id}) ID({vehicleModel.VehicleHandle.Id})");
        }
    }
}
