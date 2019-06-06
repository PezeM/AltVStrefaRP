using System.Collections.Generic;
using System.Linq;
using AltV.Net.Data;
using AltVStrefaRPServer.Models;
using VehicleModel = AltV.Net.Enums.VehicleModel;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsManager
    {
        public List<VehicleShop> VehicleShops { get; private set; }

        public VehicleShopsManager()
        {
            VehicleShops = CreateVehicleShops();
        }

        public VehicleShop GetVehicleShop(int shopId) => VehicleShops.FirstOrDefault(s => s.VehicleShopId == shopId);

        public VehicleShop GetClosestVehicleShop(Position position, int range = 10) 
            => VehicleShops.FirstOrDefault(s => s.Position.Distance(position) < range);

        public bool IsNearVehicleShop(Position position, out VehicleShop vehicleShop, int range = 10)
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
                        new VehiclePrice(100, VehicleModel.Alpha),
                        new VehiclePrice(150, VehicleModel.Banshee),
                        new VehiclePrice(150, VehicleModel.Zentorno),
                        new VehiclePrice(150, VehicleModel.Schafter6),
                        new VehiclePrice(150, VehicleModel.Seven70),
                        new VehiclePrice(150, VehicleModel.T20),
                        new VehiclePrice(150, VehicleModel.Torero),
                        new VehiclePrice(150, VehicleModel.Baller2),
                        new VehiclePrice(150, VehicleModel.Baller5),
                        new VehiclePrice(150, VehicleModel.CogCabrio),
                        new VehiclePrice(150, VehicleModel.Cog552),
                    }, new Position(-35, -1103, 26), new Rotation(0,0,0)),
                // Planes
                new VehicleShop(2, new Position(-986.62817f, -2947.9396f, 13.9450f), 
                    new List<VehiclePrice>
                    {
                        new VehiclePrice(100000, VehicleModel.AlphaZ1),
                        new VehiclePrice(100000, VehicleModel.Cuban800),
                        new VehiclePrice(80000, VehicleModel.Duster),
                        new VehiclePrice(100000, VehicleModel.Velum),
                        new VehiclePrice(150000, VehicleModel.Velum2),
                        new VehiclePrice(100000, VehicleModel.Vestra),
                    }, new Position(-986.62817f, -2947.9396f, 13.9450f), new Rotation(0,0,0)),
                // Classic cars
                new VehicleShop(3, new Position(-182.4818f, -1383.7307f, 31.2663f), 
                    new List<VehiclePrice>
                    {
                        new VehiclePrice(100, VehicleModel.Adder)
                    }, new Position(-182.4818f, -1383.7307f, 31.2663f), new Rotation(0,0,0))
            };
        }
    }
}
