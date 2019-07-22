using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
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
            using (var context = _factory())
            {
                return context.VehicleShops
                    .Include(q => q.AvailableVehicles).ToList();
            }
        }

        public async Task SaveVehicleShopAsync(VehicleShop shop)
        {
            using (var context = _factory.Invoke())
            {
                context.VehicleShops.Update(shop);
                await context.SaveChangesAsync();
            }
        }

        public void AddNewVehicleShop(VehicleShop shop)
        {
            using (var context = _factory.Invoke())
            {
                context.VehicleShops.Add(shop);
                context.SaveChanges();
            }
        }

    }
}
