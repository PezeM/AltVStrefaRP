using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleLoader : IVehicleLoader
    {
        private ServerContext _serverContext;

        public VehicleLoader(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public async Task LoadVehiclesFromDatabaseAsync()
        {
            VehicleManager.Instance.AddVehicles(await _serverContext.Vehicles.ToListAsync().ConfigureAwait(false));
        }
    }
}
