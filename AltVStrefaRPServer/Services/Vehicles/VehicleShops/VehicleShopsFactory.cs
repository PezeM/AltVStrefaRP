using AltV.Net.Data;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Vehicle;
using System.Collections.Generic;
using VehicleModel = AltV.Net.Enums.VehicleModel;

namespace AltVStrefaRPServer.Services.Vehicles.VehicleShops
{
    public class VehicleShopsFactory : IVehicleShopsFactory
    {
        public IEnumerable<VehicleShop> CreateDefaultVehicleShops(IVehicleShopDatabaseService vehicleShopDatabaseService)
        {
            var luxuryVehicleShop = CreateLuxuryVehicleShop();
            vehicleShopDatabaseService.AddNewVehicleShop(luxuryVehicleShop);

            var planesShop = CreatePlanesShop();
            vehicleShopDatabaseService.AddNewVehicleShop(planesShop);

            var classicCarShop = CreateClassicCarsShop();
            vehicleShopDatabaseService.AddNewVehicleShop(classicCarShop);

            return new List<VehicleShop>
            {
                luxuryVehicleShop, planesShop, classicCarShop
            };
        }

        private static VehicleShop CreateLuxuryVehicleShop()
        {
            return new VehicleShop(1, new Position(-35, -1103, 26),
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
                }, new Position(-35, -1103, 26), new Rotation(0, 0, 0));
        }

        private static VehicleShop CreatePlanesShop()
        {
            return new VehicleShop(2, new Position(-986.62817f, -2947.9396f, 13.9450f),
                new List<VehiclePrice>
                {
                    new VehiclePrice(100000, VehicleModel.AlphaZ1),
                    new VehiclePrice(100000, VehicleModel.Cuban800),
                    new VehiclePrice(80000, VehicleModel.Duster),
                    new VehiclePrice(100000, VehicleModel.Velum),
                    new VehiclePrice(150000, VehicleModel.Velum2),
                    new VehiclePrice(100000, VehicleModel.Vestra),
                }, new Position(-986.62817f, -2947.9396f, 13.9450f), new Rotation(0, 0, 0));
        }

        private static VehicleShop CreateClassicCarsShop()
        {
            return new VehicleShop(3, new Position(-182.4818f, -1383.7307f, 31.2663f),
                new List<VehiclePrice>
                {
                    new VehiclePrice(100, VehicleModel.Adder)
                }, new Position(-182.4818f, -1383.7307f, 31.2663f), new Rotation(0, 0, 0));
        }
    }
}
