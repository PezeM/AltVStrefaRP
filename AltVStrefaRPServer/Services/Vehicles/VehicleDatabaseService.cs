using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleDatabaseService : IVehicleDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public VehicleDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<VehicleModel> LoadVehiclesFromDatabase()
        {
            using (var context = _factory.Invoke())
            {
                return context.Vehicles;
            }
        }

        public Task RemoveVehicleAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Remove(vehicle);
                return context.SaveChangesAsync();
            }
        }

        public void SaveVehicle(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Update(vehicle);
                context.SaveChanges();
            }
        }

        public Task SaveVehicleAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Update(vehicle);
                return context.SaveChangesAsync();
            }
        }

        public void AddVehicleToDatabase(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }
        }

        public Task AddVehicleToDatabaseAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.AddAsync(vehicle);
                return context.SaveChangesAsync();
            }
        }
    }
}
