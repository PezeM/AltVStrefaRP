﻿using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Handlers
{
    public class VehicleHandler
    {
        private ServerContext _serverContext;
        private VehicleManager _vehicleManager;
        private INotificationService _notificationService;

        public VehicleHandler(ServerContext serverContext, VehicleManager vehicleManager, INotificationService notificationService)
        {
            _serverContext = serverContext;
            _vehicleManager = vehicleManager;
            _notificationService = notificationService;

            AltAsync.OnPlayerLeaveVehicle += OnPlayerLeaveVehicleAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnVehicleRemove += OnVehicleRemoveAsync;
            AltAsync.OnPlayerChangeVehicleSeat += OnPlayerChangedVehicleSeatAsync;
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
            var vehicleModel = _vehicleManager.GetVehicleModel(vehicle.Id);
            if (vehicleModel == null) return;

            // For now saves vehicle when player leaves the vehicle and he was the driver
            vehicleModel.X = vehicle.Position.X;
            vehicleModel.Y = vehicle.Position.Y;
            vehicleModel.Z = vehicle.Position.Z;
            vehicleModel.Dimension = vehicle.Dimension;

            _serverContext.Vehicles.Update(vehicleModel);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
            AltAsync.Log($"Saved vehicle {vehicleModel.Model} UID({vehicleModel.Id}) in {Time.GetTimestampMs() - startTime}ms.");
            _notificationService.ShowInfoNotification(player, 
                $"Zapisano pojazd UID({vehicleModel.Id}) w {Time.GetTimestampMs() - startTime}ms.");
        }
    }
}
