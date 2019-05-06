using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleDatabaseService : IVehicleDatabaseService
    {
        private readonly ServerContext _serverContext;

        public VehicleDatabaseService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        /// <summary>
        /// Gets all vehicles from database
        /// </summary>
        /// <returns></returns>
        public Task<List<VehicleModel>> LoadVehiclesFromDatabaseAsync()
            => _serverContext.Vehicles.ToListAsync();

        public List<VehicleModel> LoadVehiclesFromDatabase()
            => _serverContext.Vehicles.ToList();

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
    }
}
