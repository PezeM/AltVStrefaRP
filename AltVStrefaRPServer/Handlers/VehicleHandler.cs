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
        private VehicleManager _vehicleManager;
        private INotificationService _notificationService;
        private IVehicleSpawnService _vehicleSpawnService;

        public VehicleHandler(VehicleManager vehicleManager, IVehicleDatabaseService vehiceVehicleDatabaseService,
            INotificationService notificationService, IVehicleSpawnService vehicleSpawnService)
        {
            _vehicleDatabaseService = vehiceVehicleDatabaseService;
            _vehicleManager = vehicleManager;
            _notificationService = notificationService;
            _vehicleSpawnService = vehicleSpawnService;

            AltAsync.OnPlayerLeaveVehicle += OnPlayerLeaveVehicleAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnVehicleRemove += OnVehicleRemoveAsync;
            AltAsync.OnPlayerChangeVehicleSeat += OnPlayerChangedVehicleSeatAsync;
            Alt.On<IPlayer>("ToggleLockState", ToggleLockStateEvent);
            Alt.On<IPlayer>("ToggleHoodState", ToggleHoodStateEvent);
            Alt.On<IPlayer>("ToggleTrunkState", ToggleTrunkStateEvent);
            Alt.On<IPlayer, IMyVehicle>("ToggleVehicleEngine", ToggleVehicleEngineEvent);
            Alt.On<IPlayer, IMyVehicle>("TryToOpenVehicle", TryToOpenVehicleEvent);
            AltAsync.On<IPlayer, int>("DespawnVehicle", async (player, vehicleId) => await DespawnVehicleEvent(player, vehicleId));
        }

        private void ToggleLockStateEvent(IPlayer player)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            var closestVehicle = VehicleHelper.GetClosestVehicle(player, 6f);
            if (closestVehicle == null) return;
            if (!_vehicleManager.GetVehicleModel(closestVehicle, out VehicleModel vehicle)) return;
            if (!_vehicleManager.HasVehiclePermission(character, vehicle)) return;

            vehicle.IsLocked = !vehicle.IsLocked;
            vehicle.VehicleHandle.LockState = vehicle.IsLocked ? VehicleLockState.Locked : VehicleLockState.Unlocked;
            player.Emit("toggleLockState", vehicle.IsLocked);
        }

        private void ToggleHoodStateEvent(IPlayer player)
        {
            var closestVehicle = VehicleHelper.GetClosestVehicle(player, 4f);
            if (closestVehicle == null || closestVehicle.LockState == VehicleLockState.Locked) return;
            if (!_vehicleManager.GetVehicleModel(closestVehicle, out VehicleModel vehicleModel)) return;

            if (vehicleModel.VehicleHandle.GetDoorState(VehicleDoor.Hood) == VehicleDoorState.Closed)
            {
                vehicleModel.VehicleHandle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleHoodState", 1);
            }
            else 
            {
                vehicleModel.VehicleHandle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.Closed);
                player.Emit("toggleHoodState", 0);
            }
        }

        private void ToggleTrunkStateEvent(IPlayer player)
        {
            var closestVehicle = VehicleHelper.GetClosestVehicle(player, 4f);
            if (closestVehicle == null || closestVehicle.LockState == VehicleLockState.Locked) return;
            if (!_vehicleManager.GetVehicleModel(closestVehicle, out VehicleModel vehicleModel)) return;

            var doorState = vehicleModel.VehicleHandle.GetDoorState(VehicleDoor.Trunk);
            if (doorState == VehicleDoorState.Closed)
            {
                vehicleModel.VehicleHandle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleTrunkState", 1);
            }
            else 
            {
                vehicleModel.VehicleHandle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.Closed);
                player.Emit("toggleTrunkState", 0);
            }
        }

        private void ToggleVehicleEngineEvent(IPlayer player, IMyVehicle vehicle)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            if (player.Seat != 1) return;

            if (!_vehicleManager.GetVehicleModel(vehicle, out VehicleModel vehicleModel)) return;

            if (_vehicleManager.HasVehiclePermission(character, vehicleModel))
            {
                Alt.Log($"Enginge vehicle status: {vehicle.EngineOn}");
                vehicle.EngineOn = !vehicle.EngineOn;
                Alt.Log($"Enginge vehicle status: {vehicle.EngineOn}");
            }
            else
            {
                _notificationService.ShowErrorNotfication(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.", 5500);
            }
        }

        private void TryToOpenVehicleEvent(IPlayer player, IMyVehicle myVehicle)
        {
            var character = player.GetCharacter();
            if(character == null) return;

            if (!_vehicleManager.GetVehicleModel(myVehicle, out VehicleModel vehicleModel)) return;

            if (!_vehicleManager.HasVehiclePermission(character, vehicleModel))
            {
                _notificationService.ShowErrorNotfication(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.");
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

            var vehicle = _vehicleManager.GetVehicleModel((ushort)vehicleId);
            if(vehicle == null) return;

            if(!_vehicleManager.HasVehiclePermission(character, vehicle))
            {
                _notificationService.ShowErrorNotfication(player, "Brak kluczyków", "Nie posiadasz kluczyków do tego pojazdu.");
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
            if(vehicle.Driver != null) return;
            if (!_vehicleManager.GetVehicleModel(vehicle, out VehicleModel vehicleModel)) return;

            // For now saves vehicle when player leaves the vehicle and he was the driver
            vehicleModel.X = vehicle.Position.X;
            vehicleModel.Y = vehicle.Position.Y;
            vehicleModel.Z = vehicle.Position.Z;
            vehicleModel.Dimension = vehicle.Dimension;
            
            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel));
            await _notificationService.ShowInfoNotificationAsync(player, "Pojazd zapisany!",
                $"Zapisano pojazd UID({vehicleModel.Id}) w {Time.GetTimestampMs() - startTime}ms.");
            AltAsync.Log($"Saved vehicle {vehicleModel.Model} UID({vehicleModel.Id}) in {Time.GetTimestampMs() - startTime}ms.");
        }
    }
}
