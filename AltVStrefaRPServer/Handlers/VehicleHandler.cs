using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Vehicles;

namespace AltVStrefaRPServer.Handlers
{
    public class VehicleHandler
    {
        private IVehicleDatabaseService _vehicleDatabaseService;
        private VehicleManager _vehicleManager;
        private INotificationService _notificationService;

        public VehicleHandler(VehicleManager vehicleManager, IVehicleDatabaseService vehiceVehicleDatabaseService,
            INotificationService notificationService)
        {
            _vehicleDatabaseService = vehiceVehicleDatabaseService;
            _vehicleManager = vehicleManager;
            _notificationService = notificationService;

            AltAsync.OnPlayerLeaveVehicle += OnPlayerLeaveVehicleAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnVehicleRemove += OnVehicleRemoveAsync;
            AltAsync.OnPlayerChangeVehicleSeat += OnPlayerChangedVehicleSeatAsync;
            AltAsync.On<IPlayer>("ToggleLockState", async (player) => await ToggleLockStateEvent(player));
            AltAsync.On<IPlayer>("ToggleHoodState", async (player) => await ToggleHoodStateEvent(player));
            AltAsync.On<IPlayer>("ToggleTrunkState", async (player) => await ToggleTrunkStateEvent(player));
        }

        private Task ToggleLockStateEvent(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return Task.CompletedTask;

            var closestVehicle = _vehicleManager.GetClosestVehicle(player, 6f);
            if (closestVehicle == null) return Task.CompletedTask;

            if (!_vehicleManager.HasVehiclePermission(character, closestVehicle)) return Task.CompletedTask;

            closestVehicle.IsLocked = !closestVehicle.IsLocked;
            closestVehicle.VehicleHandle.LockState = closestVehicle.IsLocked ? VehicleLockState.Locked : VehicleLockState.Unlocked;
            player.Emit("toggleLockState", closestVehicle.IsLocked);
            Alt.Log($"ToggleLockState completed in {Time.GetTimestampMs() - startTime}ms.");
            return Task.CompletedTask;
        }

        private Task ToggleHoodStateEvent(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();

            var closestVehicle = _vehicleManager.GetClosestVehicle(player, 4f);
            if (closestVehicle == null || closestVehicle.VehicleHandle.LockState == VehicleLockState.Locked) return Task.CompletedTask;

            var doorState = closestVehicle.VehicleHandle.GetDoorState(VehicleDoor.Hood);
            if (doorState == VehicleDoorState.Closed)
            {
                closestVehicle.VehicleHandle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleHoodState", 1);
            }
            else 
            {
                closestVehicle.VehicleHandle.SetDoorState(VehicleDoor.Hood, VehicleDoorState.Closed);
                player.Emit("toggleHoodState", 0);
            }

            Alt.Log($"ToggleTrunkState completed in {Time.GetTimestampMs() - startTime}ms.");
            return Task.CompletedTask;
        }

        private Task ToggleTrunkStateEvent(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();

            var closestVehicle = _vehicleManager.GetClosestVehicle(player, 4f);
            if (closestVehicle == null || closestVehicle.VehicleHandle.LockState == VehicleLockState.Locked) return Task.CompletedTask;

            var doorState = closestVehicle.VehicleHandle.GetDoorState(VehicleDoor.Trunk);
            if (doorState == VehicleDoorState.Closed)
            {
                closestVehicle.VehicleHandle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.OpenedLevel7);
                player.Emit("toggleTrunkState", 1);
            }
            else 
            {
                closestVehicle.VehicleHandle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.Closed);
                player.Emit("toggleTrunkState", 0);
            }

            Alt.Log($"ToggleTrunkState completed in {Time.GetTimestampMs() - startTime}ms.");
            return Task.CompletedTask;
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
            var trunk = vehicle.GetDoorState(VehicleDoor.Trunk);
            if (trunk == VehicleDoorState.Closed)
            {
                vehicle.SetDoorState(VehicleDoor.Trunk, VehicleDoorState.OpenedLevel7);
            }
            return Task.CompletedTask;
        }

        private async Task OnPlayerLeaveVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
        {
            var startTime = Time.GetTimestampMs();
            // Saves vehicle only if the drivers exits
            if(vehicle.Driver != null) return;
            var vehicleModel = _vehicleManager.GetVehicleModel(vehicle);
            if (vehicleModel == null) return;

            // For now saves vehicle when player leaves the vehicle and he was the driver
            vehicleModel.X = vehicle.Position.X;
            vehicleModel.Y = vehicle.Position.Y;
            vehicleModel.Z = vehicle.Position.Z;
            vehicleModel.Dimension = vehicle.Dimension;

            await _vehicleDatabaseService.SaveVehicleAsync(vehicleModel).ConfigureAwait(false);
            _notificationService.ShowInfoNotification(player, "Pojazd zapisany!",
                $"Zapisano pojazd UID({vehicleModel.Id}) w {Time.GetTimestampMs() - startTime}ms.");
            AltAsync.Log($"Saved vehicle {vehicleModel.Model} UID({vehicleModel.Id}) in {Time.GetTimestampMs() - startTime}ms.");
        }
    }
}
