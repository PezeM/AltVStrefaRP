using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Vehicles;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Vehicles;
using Microsoft.Extensions.Logging;
using VehicleModel = AltVStrefaRPServer.Models.Vehicles.VehicleModel;

namespace AltVStrefaRPServer.Handlers
{
    public class VehicleHandler
    {
        private readonly IVehicleDatabaseService _vehicleDatabaseService;
        private readonly IVehiclesManager _vehiclesManager;
        private readonly INotificationService _notificationService;
        private readonly IVehicleSpawnService _vehicleSpawnService;
        private readonly ILogger<VehicleHandler> _logger;

        public VehicleHandler(IVehiclesManager vehiclesManager, IVehicleDatabaseService vehiceVehicleDatabaseService,
            INotificationService notificationService, IVehicleSpawnService vehicleSpawnService, ILogger<VehicleHandler> logger)
        {
            _vehicleDatabaseService = vehiceVehicleDatabaseService;
            _vehiclesManager = vehiclesManager;
            _notificationService = notificationService;
            _vehicleSpawnService = vehicleSpawnService;
            _logger = logger;   

            AltAsync.OnPlayerLeaveVehicle += OnPlayerLeaveVehicleAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnVehicleRemove += OnVehicleRemoveAsync;
            AltAsync.OnPlayerChangeVehicleSeat += OnPlayerChangedVehicleSeatAsync;
            Alt.On<IPlayer, IVehicle>("ToggleLockState", ToggleLockStateEvent);
            Alt.On<IPlayer, IMyVehicle>("ToggleVehicleEngine", ToggleVehicleEngineEvent);
            Alt.On<IPlayer, IMyVehicle>("TryToOpenVehicle", TryToOpenVehicleEvent);
            Alt.On<IPlayer, IMyVehicle>("ToggleTrunkState", ToggleTrunkState);
            Alt.On<IPlayer, IMyVehicle>("ToggleHoodState", ToggleHoodState);
            AltAsync.On<IPlayer, int, Task>("DespawnVehicle", DespawnVehicleEventAsync);
        }

        private void ToggleLockStateEvent(IPlayer player, IVehicle closestVehicle)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!_vehiclesManager.TryGetVehicleModel(closestVehicle, out VehicleModel vehicle)) return;
            if (!_vehiclesManager.HasVehiclePermission(character, vehicle)) return;

            vehicle.IsLocked = !vehicle.IsLocked;
            vehicle.VehicleHandle.LockState = vehicle.IsLocked ? VehicleLockState.Locked : VehicleLockState.Unlocked;
            player.Emit("toggleLockState", closestVehicle);
        }

        private void ToggleHoodState(IPlayer player, IMyVehicle vehicle)
        {
            if(vehicle.LockState == VehicleLockState.Locked) return;
            if (vehicle.GetDoorState(VehicleDoor.Hood) == VehicleDoorState.Closed)
            {
                vehicle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleHoodState", 1, vehicle);
            }
            else if(vehicle.GetDoorState(VehicleDoor.Hood) == VehicleDoorState.OpenedLevel7)
            {
                vehicle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.Closed);
                player.Emit("toggleHoodState", 0, vehicle);
            }
        }

        private void ToggleTrunkState(IPlayer player, IMyVehicle vehicle)
        {
            if(vehicle.LockState == VehicleLockState.Locked) return;
            if (vehicle.GetDoorState(VehicleDoor.Trunk) == VehicleDoorState.Closed)
            {
                vehicle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleTrunkState", 1, vehicle);
            }
            else if(vehicle.GetDoorState(VehicleDoor.Trunk) == VehicleDoorState.OpenedLevel7)
            {
                vehicle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.Closed);
                player.Emit("toggleTrunkState", 0, vehicle);
            }
        }

        private void ToggleVehicleEngineEvent(IPlayer player, IMyVehicle vehicle)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (player.Seat != 1) return;

            if (!_vehiclesManager.TryGetVehicleModel(vehicle, out VehicleModel vehicleModel)) return;

            if (_vehiclesManager.HasVehiclePermission(character, vehicleModel))
            {
                vehicle.EngineOn = !vehicle.EngineOn;
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.", 5500);
            }
        }

        private void TryToOpenVehicleEvent(IPlayer player, IMyVehicle myVehicle)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            if (!_vehiclesManager.TryGetVehicleModel(myVehicle, out VehicleModel vehicleModel)) return;
            if (!_vehiclesManager.HasVehiclePermission(character, vehicleModel))
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.");
                return;
            }

            vehicleModel.IsLocked = !vehicleModel.IsLocked;
            myVehicle.LockState = vehicleModel.IsLocked ? VehicleLockState.Locked : VehicleLockState.Unlocked;

            player.Emit("toggleLockState", myVehicle);
        }

        private async Task DespawnVehicleEventAsync(IPlayer player, int vehicleId)
        {
            var character = player.GetCharacter();
            if(character == null) return;

            var vehicle = _vehiclesManager.GetVehicleModel((ushort)vehicleId);
            if(vehicle == null) return;

            _logger.LogDebug("Character {characterId} tried to open vehicle VID({vehicleId)", character.Id, vehicle.Id);
            if(!_vehiclesManager.HasVehiclePermission(character, vehicle))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.");
                return;
            }

            await _vehicleSpawnService.DespawnVehicleAsync(vehicle);
        }

        private Task OnPlayerChangedVehicleSeatAsync(IVehicle vehicle, IPlayer player, byte oldseat, byte newseat)
        {
            return Task.CompletedTask;
        }

        private Task OnVehicleRemoveAsync(IVehicle vehicle)
        {
            _logger.LogInformation("Vehicle {vehicleHandleId} was removed from the server", vehicle.Id);
            return Task.CompletedTask;
        }

        private Task OnPlayerEnterVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
        {
            return Task.CompletedTask;
        }

        private async Task OnPlayerLeaveVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
        {
            var startTime = Time.GetTimestampMs();
            // Saves vehicle only if the drivers exits
            if(vehicle.Driver != player) return;
            if (!_vehiclesManager.TryGetVehicleModel(vehicle, out VehicleModel vehicleModel)) return;

            // For thread safety
            await AltAsync.Do(() =>
            {
                vehicleModel.X = vehicle.Position.X;
                vehicleModel.Y = vehicle.Position.Y;
                vehicleModel.Z = vehicle.Position.Z;
                vehicleModel.Dimension = vehicle.Dimension;
            }).ConfigureAwait(false);

            // For now saves vehicle when player leaves the vehicle and he was the driver
            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel).ConfigureAwait(false);
            await _notificationService.ShowInfoNotificationAsync(player, "Pojazd zapisany!",
                $"Zapisano pojazd UID({vehicleModel.Id}) w {Time.GetElapsedTime(startTime)}ms.");
            _logger.LogDebug("Saved vehicle VID({vehicleId}) to database in {elapsedTime}ms", vehicleModel.Id, Time.GetElapsedTime(startTime));
        }
    }
}
