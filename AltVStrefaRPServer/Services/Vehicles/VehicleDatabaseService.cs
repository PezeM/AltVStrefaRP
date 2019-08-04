using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Vehicles;

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
            using (var context = _factory())
            {
                return context.Vehicles.ToList();
            }
        }

        public async Task RemoveVehicleAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Remove(vehicle);
                await context.SaveChangesAsync();
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

        public async Task SaveVehicleAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync();
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

        public async Task AddVehicleToDatabaseAsync(VehicleModel vehicle)
        {
            using (var context = _factory.Invoke())
            {
                await context.Vehicles.AddAsync(vehicle);
                await context.SaveChangesAsync();
            }
        }
    }
}
