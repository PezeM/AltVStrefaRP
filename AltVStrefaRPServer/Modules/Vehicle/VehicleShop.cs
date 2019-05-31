using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShop
    {
        public int VehicleShopId { get; set; }
        public Position Position { get; set; }
        public Position PositionOfBoughtVehicles { get; set; }
        public Rotation RotationOfBoughtVehicles { get; set; }
        public List<VehiclePrice> AvailableVehicles { get; set; }
        public IBlip ShopBlip { get; set; }
        public int BlipSprite { get; set; }
        public int BlipColor { get; set; }

        public VehicleShop(int vehicleShopId, Position position, List<VehiclePrice> avaibleVehicles, Position positionOfBoughtVehicles, 
            Rotation rotationOfBoughtVehicles, int blipSprite = 67, int blipColor = 1)
        {
            VehicleShopId = vehicleShopId;
            Position = position;
            PositionOfBoughtVehicles = positionOfBoughtVehicles;
            RotationOfBoughtVehicles = rotationOfBoughtVehicles;
            AvailableVehicles = avaibleVehicles;
            BlipColor = blipColor;
            BlipSprite = blipSprite;

            CreateShopBlip();
        }

        private void CreateShopBlip()
        {
            ShopBlip = Alt.CreateBlip(BlipType.Object, Position);
            ShopBlip.Sprite = 67;
            ShopBlip.Color = 1;
            Alt.Log($"VehicleShop blip for shop {VehicleShopId}. Type: {ShopBlip.BlipType} Position: {ShopBlip.Position}");
        }
    }
}
