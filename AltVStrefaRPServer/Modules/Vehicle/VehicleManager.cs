using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
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
        private Dictionary<int, VehicleModel> Vehicles = new Dictionary<int, VehicleModel>();
        private IVehicleDatabaseService _vehicleDatabaseService;
        private IVehicleCreatorService _vehicleCreator;
        private BusinessManager _businessManager;

        public VehicleManager(IVehicleDatabaseService vehicleDatabaseService, IVehicleCreatorService vehicleCreatorService, 
            BusinessManager businessManager)
        {
            _vehicleDatabaseService = vehicleDatabaseService;
            _vehicleCreator = vehicleCreatorService;

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
        /// Gets <see cref="VehicleModel"/> by IVehicle
        /// </summary>
        /// <param name="vehicle"><see cref="IVehicle"/></param>
        /// <returns></returns>
        public VehicleModel GetVehicleModel(IVehicle vehicle) => vehicle.GetData("vehicleId", out int vehicleId) ? Vehicles[vehicleId] : null;

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

        public bool IsCharacterOwnerOfVehicle(Character character, VehicleModel vehicle) => character.Id == vehicle.Id;

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
                // TODO Group management
                if (character.Business == null) character.Business = _businessManager.GetBusiness(character);
                if (character.Business == null) return false;
                if (!character.Business.GetBusinessRank(character.BusinessRank, out BusinessRank businessRank)) return false;
                return businessRank.Permissions.HaveVehicleKeys;
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
        /// Returns vehicles owned by character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public List<VehicleModel> GetPlayerVehicles(Models.Character character)
        {
            return Vehicles.Values.Where(v => v.Owner == character.Id && v.OwnerType == OwnerType.Character).ToList();
        }

        public async Task<VehicleModel> CreateVehicleAsync(string vehicleModel, Position position, float heading, short dimension, int ownerId, 
            OwnerType ownerType)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, heading, dimension, ownerId, ownerType);
            await _vehicleCreator.SaveVehicleToDatabaseAsync(vehicle).ConfigureAwait(false);
            Vehicles.Add(vehicle.Id, vehicle);
            Alt.Log($"Created vehicle {vehicle.Model} UID({vehicle.Id}).");
            return vehicle;
        }

        public VehicleModel CreateVehicle(string vehicleModel, Position position, float heading, short dimension, int ownerId, 
            OwnerType ownerType)
        {
            var vehicle = _vehicleCreator.CreateVehicle(vehicleModel, position, heading, dimension, ownerId, ownerType);
            _vehicleCreator.SaveVehicleToDatabase(vehicle);
            Vehicles.Add(vehicle.Id, vehicle);
            Alt.Log($"Created vehicle {vehicle.Model} UID({vehicle.Id}).");
            return vehicle;
        }

        public void SpawnVehicle(int vehicleId)
        {
            SpawnVehicle(GetVehicleModel(vehicleId));
        }

        public async Task SpawnVehicleAsync(int vehicleId)
        {
            await SpawnVehicleAsync(GetVehicleModel(vehicleId));
        }

        public async Task SpawnVehicleAsync(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return;
            if (vehicleModel.IsSpawned) return;
            if (vehicleModel.VehicleHandle != null) return;

            try
            {
                vehicleModel.VehicleHandle = await AltAsync.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z), new Rotation(vehicleModel.Heading, 0,0)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Alt.Log($"Error creating vehicle with model {vehicleModel.Model} ID({vehicleModel.Id}) ex: {e}");
                throw;
            }


            vehicleModel.VehicleHandle.Dimension = vehicleModel.Dimension;
            vehicleModel.IsLocked = false;
            vehicleModel.VehicleHandle.LockState = VehicleLockState.Unlocked;
            vehicleModel.VehicleHandle.SetData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.NumberplateText = vehicleModel.PlateText;
            vehicleModel.VehicleHandle.NumberplateIndex = vehicleModel.PlateNumber;
            vehicleModel.IsSpawned = true;

            Alt.Log($"Spawned vehicle UID({vehicleModel.Id}) ID({vehicleModel.VehicleHandle.Id})");
        }

        public void SpawnVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return;
            if (vehicleModel.IsSpawned) return;
            if (vehicleModel.VehicleHandle != null) return;

            try
            {
                vehicleModel.VehicleHandle = Alt.CreateVehicle(vehicleModel.Model,
                    new Position(vehicleModel.X, vehicleModel.Y, vehicleModel.Z), new Rotation(vehicleModel.Heading, 0,0));
            }
            catch (Exception e)
            {
                Alt.Log($"Error creating vehicle with model {vehicleModel.Model} ID({vehicleModel.Id}) ex: {e}");
                throw;
            }


            vehicleModel.VehicleHandle.Dimension = vehicleModel.Dimension;
            vehicleModel.VehicleHandle.EngineOn = true;
            vehicleModel.IsLocked = false;
            vehicleModel.VehicleHandle.LockState = VehicleLockState.Unlocked;
            vehicleModel.VehicleHandle.SetData("vehicleId", vehicleModel.Id);
            vehicleModel.VehicleHandle.NumberplateText = vehicleModel.PlateText;
            vehicleModel.VehicleHandle.NumberplateIndex = vehicleModel.PlateNumber;
            vehicleModel.IsSpawned = true;

            Alt.Log($"Spawned vehicle UID({vehicleModel.Id}) ID({vehicleModel.VehicleHandle.Id})");
        }

        public async Task<bool> DespawnVehicleAsync(int vehicleId)
        {
            var vehicle = GetVehicleModel(vehicleId);
            if (vehicle == null) return false;

            return await DespawnVehicleAsync(vehicle).ConfigureAwait(false);
        }

        /// <summary>
        /// Despawns vehicle from game and saves its to database
        /// </summary>
        /// <param name="vehicleModel"></param>
        /// <returns></returns>
        public async Task<bool> DespawnVehicleAsync(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return false;
            if (!vehicleModel.IsSpawned) return true;

            vehicleModel.IsSpawned = false;
            vehicleModel.X = vehicleModel.VehicleHandle.Position.X;
            vehicleModel.Y = vehicleModel.VehicleHandle.Position.Y;
            vehicleModel.Z = vehicleModel.VehicleHandle.Position.Z;
            vehicleModel.Dimension = vehicleModel.VehicleHandle.Dimension;
            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel).ConfigureAwait(false);
            Alt.Server.RemoveVehicle(vehicleModel.VehicleHandle);
            AltAsync.Log($"Despawned vehicle: {vehicleModel.Model} UID({vehicleModel.Id})");
            return true;
        }

        public VehicleModel GetClosestVehicle(IPlayer player, float radius = 4f)
        {
            var startTime = Time.GetTimestampMs();
            IVehicle returnVehicle = null;
            foreach (var vehicle in Alt.GetAllVehicles())
            {
                var playerDistanceToVehicle = player.Position.Distance(vehicle.Position);
                if (playerDistanceToVehicle < radius && player.Dimension == vehicle.Dimension)
                {
                    radius = playerDistanceToVehicle;
                    returnVehicle = vehicle;
                }
            }
            Alt.Log($"Found the nearest vehicle in {Time.GetTimestampMs() - startTime} ms.");

            return returnVehicle == null ? null : GetVehicleModel(returnVehicle);
        }
    }
}
