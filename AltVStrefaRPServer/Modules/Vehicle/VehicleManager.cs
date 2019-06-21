using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Services.Vehicles;
using VehicleModel = AltVStrefaRPServer.Models.VehicleModel;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleManager
    {
        private Dictionary<int, VehicleModel> _vehicles = new Dictionary<int, VehicleModel>();
        private IVehicleDatabaseService _vehicleDatabaseService;
        private IVehicleCreatorService _vehicleCreator;
        private BusinessManager _businessManager;

        public VehicleManager(IVehicleDatabaseService vehicleDatabaseService, IVehicleCreatorService vehicleCreatorService, 
            BusinessManager businessManager)
        {
            _vehicleDatabaseService = vehicleDatabaseService;
            _vehicleCreator = vehicleCreatorService;
            _businessManager = businessManager;

            LoadVehiclesFromDatabaseAsync();
        }

        /// <summary>
        /// Loads all vehicles from database to memory and sets every vehicle IsSpawned property to false
        /// </summary>
        public void LoadVehiclesFromDatabaseAsync()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var vehicle in _vehicleDatabaseService.LoadVehiclesFromDatabase())
            {
                vehicle.IsSpawned = false;
                _vehicles.Add(vehicle.Id, vehicle);
            }
            Alt.Log($"Loaded {_vehicles.Count} vehicles from database in {Time.GetTimestampMs() - startTime}ms.");
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
                    Alt.RemoveVehicle(vehicle.VehicleHandle);
                    await _vehicleDatabaseService.RemoveVehicleAsync(vehicle).ConfigureAwait(false);
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
                    return (character.Fraction?.GetEmployeePermissions(character)?.HaveVehicleKeys).Value;
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
            _vehicles.Add(vehicle.Id, vehicle);
            AltAsync.Log($"Created vehicle {vehicle.Model} UID({vehicle.Id}).");
            return vehicle;
        }

        public VehicleModel CreateVehicle(string vehicleModel, Position position, Rotation rotation, short dimension, int ownerId, 
            OwnerType ownerType)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, rotation, dimension, ownerId, ownerType);
            _vehicleDatabaseService.AddVehicleToDatabase(vehicle);
            _vehicles.Add(vehicle.Id, vehicle);
            Alt.Log($"Created vehicle {vehicle.Model} UID({vehicle.Id}).");
            return vehicle;
        }
    }
}
