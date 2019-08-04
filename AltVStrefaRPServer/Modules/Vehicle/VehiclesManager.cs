using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Vehicles;
using AltVStrefaRPServer.Services.Vehicles;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehiclesManager : IVehiclesManager
    {
        private Dictionary<int, VehicleModel> _vehicles;
        private IVehicleDatabaseService _vehicleDatabaseService;
        private IVehicleCreatorService _vehicleCreator;
        private readonly ILogger<VehiclesManager> _logger;

        public VehiclesManager(IVehicleDatabaseService vehicleDatabaseService, IVehicleCreatorService vehicleCreatorService, ILogger<VehiclesManager> logger)
        {
            _vehicles = new Dictionary<int, VehicleModel>();
            _vehicleDatabaseService = vehicleDatabaseService;
            _vehicleCreator = vehicleCreatorService;
            _logger = logger;

            LoadVehiclesFromDatabase();
        }

        /// <summary>
        /// Gets VehicleModel by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>Returns <see cref="VehicleModel"/></returns>
        public bool TryGetVehicleModel(int vehicleId, out VehicleModel vehicleModel) => _vehicles.TryGetValue(vehicleId, out vehicleModel);

        /// <summary>
        /// Gets VehicleModel by vehicleHandle id
        /// </summary>
        /// <param name="vehicleID">Id of vehicle handle</param>
        /// <returns></returns>
        public VehicleModel GetVehicleModel(ushort vehicleID) => _vehicles.Values.FirstOrDefault(v => v.VehicleHandle?.Id == vehicleID);

        public bool TryGetVehicleModel(IMyVehicle vehicle, out VehicleModel vehicleModel) 
            => _vehicles.TryGetValue(vehicle.DatabaseId, out vehicleModel);

        /// <summary>
        /// Gets <see cref="VehicleModel"/> by IVehicle
        /// </summary>
        /// <param name="vehicle"><see cref="IVehicle"/></param>
        /// <returns></returns>
        public bool TryGetVehicleModel(IVehicle vehicle, out VehicleModel vehicleModel)
        {
            vehicleModel = null;
            if (!vehicle.GetData("vehicleId", out int vehicleId)) return false;
            return _vehicles.TryGetValue(vehicleId, out vehicleModel);
        }

        /// <summary>
        /// Removes <see cref="VehicleModel"/> from vehicle list by id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>True if vehicle was removed successfully</returns>
        public bool RemoveVehicle(int vehicleId) => _vehicles.Remove(vehicleId);

        /// <summary>
        /// Removes <see cref="VehicleModel"/> from vehicle list by value
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>True if vehicle was removed successfully</returns>
        public bool RemoveVehicle(VehicleModel vehicle) => _vehicles.Remove(vehicle.Id);

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
                    lock (vehicle.VehicleHandle)
                    {
                        if (vehicle.VehicleHandle.Exists)
                        {
                            Alt.RemoveVehicle(vehicle.VehicleHandle);
                        }
                    }
                    await _vehicleDatabaseService.RemoveVehicleAsync(vehicle).ConfigureAwait(false);
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in removing vehicle from world. Vehicle {@vehicle}", vehicle);
                    throw;
                }
            }
            return false;
        }

        public bool IsCharacterOwnerOfVehicle(Character character, VehicleModel vehicle) => character.Id == vehicle.Owner;

        /// <summary>
        /// Checks if character has permission to access vehicle
        /// </summary>
        /// <param name="character">Character which wants to acess vehicle</param>
        /// <param name="vehicle">The vehicle character want to access</param>
        /// <returns>True if character has permission</returns>
        public bool HasVehiclePermission(Character character, VehicleModel vehicle)
        {
            if (vehicle.OwnerType == OwnerType.Character)
            {
                return vehicle.Owner == character.Id;
            }
            else if (vehicle.OwnerType == OwnerType.Group)
            {
                if (character.CurrentBusinessId > 0)
                {
                    if (character.Business == null) return false;
                    else
                    {
                        if (!character.Business.GetBusinessRankForEmployee(character, out BusinessRank rank)) return false;
                        return rank.Permissions.HaveVehicleKeys;
                    }
                }
                else if (character.CurrentFractionId > 0)
                {
                    return character.Fraction?.HasPermission<VehiclePermission>(character) ?? false;
                }
                else
                {
                    return false;
                }
            }
            else if (vehicle.OwnerType == OwnerType.Job)
            {
                return vehicle.Owner == character.Id;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns all vehicles owned by character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public List<VehicleModel> GetAllPlayerVehicles(Character character)
            => _vehicles.Values.Where(v => v.Owner == character.Id && v.OwnerType == OwnerType.Character).ToList();

        public async Task<VehicleModel> CreateVehicleAsync(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId,
            OwnerType ownerType)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, rotation, dimension, ownerId, ownerType);
            await _vehicleDatabaseService.AddVehicleToDatabaseAsync(vehicle);
            lock (_vehicles)
            {
                _vehicles.Add(vehicle.Id, vehicle);
            }
            _logger.LogInformation("Created vehicle {vehicleModel} ID({vehicleId}) by CID({characterId})", vehicleModel, vehicle.Id, ownerId);
            return vehicle;
        }

        public VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, 
            OwnerType ownerType)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, rotation, dimension, ownerId, ownerType);
            _vehicleDatabaseService.AddVehicleToDatabase(vehicle);
            _vehicles.Add(vehicle.Id, vehicle);
            _logger.LogInformation("Created vehicle {vehicleModel} VID({vehicleId}) by CID({characterId})", vehicleModel, vehicle.Id, ownerId);
            return vehicle;
        }

        private void LoadVehiclesFromDatabase()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicle in _vehicleDatabaseService.LoadVehiclesFromDatabase())
            {
                vehicle.IsSpawned = false;
                _vehicles.Add(vehicle.Id, vehicle);
            }
            _logger.LogInformation("Loaded {vehiclesCount} vehicles from databse in {elapsedTime} ms", _vehicles.Count, Time.GetElapsedTime(startTime));
        }
    }
}
