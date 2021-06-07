using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Vehicles;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleMileageHandler
    {
        private readonly ILogger<VehicleMileageHandler> _logger;

        public VehicleMileageHandler(ILogger<VehicleMileageHandler> logger)
        {
            _logger = logger;
            Alt.On<IPlayer, IMyVehicle, float>("Vehicle-AddMileage", AddMileage);
        }

        private void OnAddMileage(IPlayer player, object[] args)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (player.Vehicle == null || !player.Vehicle.Exists) return;
            var hashDistance = Convert.ToSingle(args[0]);
            _logger.LogDebug("Hashdistance is {hashDistance}", hashDistance);
            if (!(player.Vehicle is IMyVehicle playerVehicle)) return;
            playerVehicle.Mileage += hashDistance;
            _logger.LogInformation("New vehicle ID({vehicleId}) mileage is set to {vehicleMileage}", playerVehicle.DatabaseId, playerVehicle.Mileage);
        }

        private void AddMileage(IPlayer player, IMyVehicle vehicle, float hashDistance)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (vehicle == null || !vehicle.Exists) return;
            _logger.LogDebug("Hashdistance is {hashDistance}", hashDistance);
            vehicle.Mileage += hashDistance;
            _logger.LogInformation("New vehicle ID({vehicleId}) mileage is set to {vehicleMileage}", vehicle.DatabaseId, vehicle.Mileage);
        }
    }
}
