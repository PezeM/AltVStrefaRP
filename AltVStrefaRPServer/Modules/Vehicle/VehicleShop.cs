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
        public Position CameraPosition { get; set; }
        public Rotation CameraRotation { get; set; }
        public List<VehiclePrice> AvailableVehicles { get; set; }
        public IBlip ShopBlip { get; set; }
        public int BlipSprite { get; set; }
        public int BlipColor { get; set; }

        public VehicleShop(int vehicleShopId, Position position, List<VehiclePrice> avaibleVehicles, Position cameraPosition, 
            Rotation cameraRotation = new Rotation(), int blipSprite = 67, int blipColor = 1)
        {
            Position = position;
            VehicleShopId = vehicleShopId;
            AvailableVehicles = avaibleVehicles;
            CameraPosition = cameraPosition;
            CameraRotation = cameraRotation;
            BlipColor = blipColor;
            BlipSprite = blipSprite;

            CreateShopBlip();
        }

        private void CreateShopBlip()
        {
            ShopBlip = Alt.CreateBlip(BlipType.Object, Position);
            ShopBlip.Sprite = 67;
            ShopBlip.Color = 1;
        }
    }
}
