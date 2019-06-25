using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleDatabaseService : IVehicleDatabaseService
    {
        private readonly ServerContext _serverContext;

        public VehicleDatabaseService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public IEnumerable<VehicleModel> LoadVehiclesFromDatabase()
            => _serverContext.Vehicles;

        public Task RemoveVehicleAsync(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Remove(vehicle);
            return _serverContext.SaveChangesAsync();
        }

        public void SaveVehicle(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Update(vehicle);
            _serverContext.SaveChanges();
        }

        public Task SaveVehicleAsync(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Update(vehicle);
            return _serverContext.SaveChangesAsync();
        }

        public void AddVehicleToDatabase(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Add(vehicle);
            _serverContext.SaveChanges();
        }

        public Task AddVehicleToDatabaseAsync(VehicleModel vehicle)
        {
            _serverContext.Vehicles.AddAsync(vehicle);
            return _serverContext.SaveChangesAsync();
        }
    }
}
