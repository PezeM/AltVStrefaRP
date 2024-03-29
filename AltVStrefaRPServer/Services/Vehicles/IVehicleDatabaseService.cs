﻿using AltVStrefaRPServer.Models.Vehicles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public interface IVehicleDatabaseService
    {
        IEnumerable<VehicleModel> LoadVehiclesFromDatabase();
        Task RemoveVehicleAsync(VehicleModel vehicle);
        void SaveVehicle(VehicleModel vehicle);
        Task SaveVehicleAsync(VehicleModel vehicle);
        void AddVehicleToDatabase(VehicleModel vehicle);
        Task AddVehicleToDatabaseAsync(VehicleModel vehicle);
    }
}
