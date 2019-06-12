using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShop : IMoney
    {
        private float _money;

        public int VehicleShopId { get; set; }
        public Position Position { get; set; }
        public Position PositionOfBoughtVehicles { get; set; }
        public Rotation RotationOfBoughtVehicles { get; set; }
        public List<VehiclePrice> AvailableVehicles { get; set; }

        public float Money
        {
            get { return _money; }
            set
            {
                _money = value;
                if(UpdateOnMoneyChange) OnMoneyChange();
            }
        }

        public IBlip ShopBlip { get; set; }
        public int BlipSprite { get; set; }
        public int BlipColor { get; set; }

        [NotMapped]
        public bool UpdateOnMoneyChange { get; } = false;

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

        public VehiclePrice FindVehicleByModel(long vehicleModel)
        {
            return AvailableVehicles.FirstOrDefault(v => (long)v.VehicleModel == vehicleModel);
        }

        private void CreateShopBlip()
        {
            ShopBlip = Alt.CreateBlip(BlipType.Object, Position);
            ShopBlip.Sprite = 67;
            ShopBlip.Color = 1;
            Alt.Log($"VehicleShop blip for shop {VehicleShopId}. Type: {ShopBlip.BlipType} Position: {ShopBlip.Position}");
        }

        public string MoneyTransactionDisplayName() => $"VehicleShop {VehicleShopId}";

        public void OnMoneyChange()
        { }
    }
}
