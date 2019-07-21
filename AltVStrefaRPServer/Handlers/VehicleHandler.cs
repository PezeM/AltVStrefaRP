using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Vehicles;
using VehicleModel = AltVStrefaRPServer.Models.VehicleModel;

namespace AltVStrefaRPServer.Handlers
{
    public class VehicleHandler
    {
        private IVehicleDatabaseService _vehicleDatabaseService;
        private VehiclesManager _vehiclesManager;
        private INotificationService _notificationService;
        private IVehicleSpawnService _vehicleSpawnService;

        public VehicleHandler(VehiclesManager vehiclesManager, IVehicleDatabaseService vehiceVehicleDatabaseService,
            INotificationService notificationService, IVehicleSpawnService vehicleSpawnService)
        {
            _vehicleDatabaseService = vehiceVehicleDatabaseService;
            _vehiclesManager = vehiclesManager;
            _notificationService = notificationService;
            _vehicleSpawnService = vehicleSpawnService;

            AltAsync.OnPlayerLeaveVehicle += OnPlayerLeaveVehicleAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnVehicleRemove += OnVehicleRemoveAsync;
            AltAsync.OnPlayerChangeVehicleSeat += OnPlayerChangedVehicleSeatAsync;
            Alt.On<IPlayer, IVehicle>("ToggleLockState", ToggleLockStateEvent);
            Alt.On<IPlayer, IMyVehicle>("ToggleVehicleEngine", ToggleVehicleEngineEvent);
            Alt.On<IPlayer, IMyVehicle>("TryToOpenVehicle", TryToOpenVehicleEvent);
            Alt.On<IPlayer, IMyVehicle>("ToggleTrunkState", ToggleTrunkState);
            Alt.On<IPlayer, IMyVehicle>("ToggleHoodState", ToggleHoodState);
            AltAsync.On<IPlayer, int>("DespawnVehicle", async (player, vehicleId) => await DespawnVehicleEvent(player, vehicleId));
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

        private async Task DespawnVehicleEvent(IPlayer player, int vehicleId)
        {
            var character = player.GetCharacter();
            if(character == null) return;

            var vehicle = _vehiclesManager.GetVehicleModel((ushort)vehicleId);
            if(vehicle == null) return;

            if(!_vehiclesManager.HasVehiclePermission(character, vehicle))
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.");
            }

            await _vehicleSpawnService.DespawnVehicleAsync(vehicle);
        }

        private Task OnPlayerChangedVehicleSeatAsync(IVehicle vehicle, IPlayer player, byte oldseat, byte newseat)
        {
            return Task.CompletedTask;
        }

        private Task OnVehicleRemoveAsync(IVehicle vehicle)
        {
            AltAsync.Log($"Vehicle {vehicle.Model} ID({vehicle.Id}) was removed from the server");
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
            });

            // For now saves vehicle when player leaves the vehicle and he was the driver
            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel);
            await _notificationService.ShowInfoNotificationAsync(player, "Pojazd zapisany!",
                $"Zapisano pojazd UID({vehicleModel.Id}) w {Time.GetTimestampMs() - startTime}ms.");
            AltAsync.Log($"Saved vehicle {vehicleModel.Model} UID({vehicleModel.Id}) in {Time.GetTimestampMs() - startTime}ms.");
        }
    }
}
