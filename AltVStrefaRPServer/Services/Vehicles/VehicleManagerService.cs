﻿using System.Collections.Generic;
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
            => await _serverContext.Vehicles.ToListAsync().ConfigureAwait(false);
    }
}
