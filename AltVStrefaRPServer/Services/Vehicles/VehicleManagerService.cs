using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleManagerService : IVehicleManagerService
    {
        private ServerContext _serverContext;

        public VehicleManagerService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public async Task<List<VehicleModel>> LoadVehiclesFromDatabaseAsync()
            => await _serverContext.Vehicles.AsNoTracking().ToListAsync().ConfigureAwait(false);

        public List<VehicleModel> LoadVehiclesFromDatabase()
            => _serverContext.Vehicles.AsNoTracking().ToList();

        public async Task RemoveVehicleAsync(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Remove(vehicle);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}
