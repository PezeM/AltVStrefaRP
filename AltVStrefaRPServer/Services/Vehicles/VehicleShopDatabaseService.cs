using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleShopDatabaseService : IVehicleShopDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public VehicleShopDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<VehicleShop> GetAllVehicleShops()
        {
            using (var context = _factory.Invoke())
            {
                return context.VehicleShops
                    .Include(q => q.AvailableVehicles);
            }
        }

        public async Task SaveVehicleShop(VehicleShop shop)
        {
            using (var context = _factory.Invoke())
            {
                context.VehicleShops.Update(shop);
                await context.SaveChangesAsync();
            }
        }
    }
}
