using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.Vehicles.VehicleShops;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShop : IMoney, IPosition, IHaveBlip
    {
        public int VehicleShopId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float BoughtVehiclesX { get; set; }
        public float BoughtVehiclesY { get; set; }
        public float BoughtVehiclesZ { get; set; }

        public float BoughtVehiclesRoll { get; set; }
        public float BoughtVehiclesPitch { get; set; }
        public float BoughtVehiclesYaw { get; set; }

        public ICollection<VehiclePrice> AvailableVehicles { get; set; }

        public float Money { get; private set; }

        public IBlip ShopBlip { get; set; }
        public int BlipSprite { get; set; }
        public int BlipColor { get; set; }
        public string BlipName => "Sklep samochodowy";

        private VehicleShop() { }

        public VehicleShop(int vehicleShopId, Position position, List<VehiclePrice> availableVehicles, Position positionOfBoughtVehicles,
            Rotation rotationOfBoughtVehicles, int blipSprite = 67, int blipColor = 1)
        {
            VehicleShopId = vehicleShopId;
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            BoughtVehiclesX = positionOfBoughtVehicles.X;
            BoughtVehiclesY = positionOfBoughtVehicles.Y;
            BoughtVehiclesZ = positionOfBoughtVehicles.Z;
            BoughtVehiclesPitch = rotationOfBoughtVehicles.Pitch;
            BoughtVehiclesRoll = rotationOfBoughtVehicles.Roll;
            BoughtVehiclesYaw = rotationOfBoughtVehicles.Yaw;
            AvailableVehicles = availableVehicles;
            BlipColor = blipColor;
            BlipSprite = blipSprite;

            CreateShopBlip();
        }

        public VehiclePrice FindVehicleByModel(long vehicleModel)
        {
            return AvailableVehicles.FirstOrDefault(v => (long)v.VehicleModel == vehicleModel);
        }

        public Position GetPosition() => new Position(X, Y, Z);

        public Position GetPositionOfBoughtVehicles() => new Position(BoughtVehiclesX, BoughtVehiclesY, BoughtVehiclesZ);

        public Rotation GetRotationOfBoughtVehicles()
            => new Rotation(BoughtVehiclesRoll, BoughtVehiclesPitch, BoughtVehiclesYaw);

        private void CreateShopBlip()
        {
            ShopBlip = Alt.CreateBlip(BlipType.Object, GetPosition());
            ShopBlip.Sprite = 67;
            ShopBlip.Color = 1;
            Alt.Log($"VehicleShop blip for shop {VehicleShopId}. Type: {ShopBlip.BlipType} Position: {ShopBlip.Position}");
        }

        public void AddMoney(float amount)
        {
            Money += amount;
        }

        public bool RemoveMoney(float amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            return true;
        }

        public string MoneyTransactionDisplayName() => $"VehicleShop {VehicleShopId}";

        public async Task<bool> AddVehicleAsync(VehiclePrice vehiclePrice, IVehicleShopDatabaseService vehicleShopDatabaseService)
        {
            if (vehiclePrice == null) return false;

            AvailableVehicles.Add(vehiclePrice);
            await vehicleShopDatabaseService.SaveVehicleShopAsync(this);
            return true;
        }
    }
}
