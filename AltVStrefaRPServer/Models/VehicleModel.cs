using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models
{
    public class VehicleModel : IPosition
    {
        public int Id { get; set; }
        public int Owner { get; set; }
        public OwnerType OwnerType { get; set; }
        public string Model { get; set; }
        public int VehicleInventoryId { get; set; }
        public VehicleInventoryController VehicleInventory { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short Dimension { get; set; }
        public float Roll { get; set; }
        public float Pitch { get; set; }
        public float Yaw { get; set; }

        public float MaxFuel { get; set; }
        public float Fuel { get; set; }
        public float MaxOil { get; set; }
        public float Oil { get; set; }
        public float Mileage { get; set; }
        public uint PlateNumber { get; set; }
        public string PlateText { get; set; }

        public bool IsSpawned { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBlocked { get; set; }

        public IVehicle VehicleHandle { get; set; }
        public bool IsJobVehicle { get; set; }

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }
    }
}
