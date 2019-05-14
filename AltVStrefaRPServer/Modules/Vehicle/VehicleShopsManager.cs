using System.Collections.Generic;
using System.Linq;
using AltV.Net.Data;
using AltVStrefaRPServer.Models;
using VehicleModel = AltV.Net.Enums.VehicleModel;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsManager
    {
        public List<VehicleShop> VehicleShops { get; set; }

        public VehicleShopsManager()
        {
            VehicleShops = CreateVehicleShops();
        }

        public VehicleShop GetClosestVehicleShop(Position position, int range = 10) 
            => VehicleShops.FirstOrDefault(s => s.Position.Distance(position) < range);

        public bool GetClosestVehicleShop(Position position, out VehicleShop vehicleShop, int range = 10)
        {
            vehicleShop = VehicleShops.FirstOrDefault(s => s.Position.Distance(position) < range);
            return vehicleShop != null;
        }

        private List<VehicleShop> CreateVehicleShops()
        {
            return new List<VehicleShop>
            {
                // Luxury vehicles
                new VehicleShop(1, new Position(-35, -1103, 26), 
                    new List<VehiclePrice>
                    {
                        new VehiclePrice(100, VehicleModel.Adder)
                    }),
                // Planes
                new VehicleShop(2, new Position(-986.62817f, -2947.9396f, 13.9450f), 
                    new List<VehiclePrice>
                    {
                        new VehiclePrice(100, VehicleModel.Adder)
                    }),
                // Classic cars
                new VehicleShop(3, new Position(-182.4818f, -1383.7307f, 31.2663f), 
                    new List<VehiclePrice>
                    {
                        new VehiclePrice(100, VehicleModel.Adder)
                    })
            };
        }
    }
}
