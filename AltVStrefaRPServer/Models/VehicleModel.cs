using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public int Owner { get; set; }
        public OwnerType OwnerType { get; set; }
        public string Model { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short Dimension { get; set; }
        public float Heading { get; set; }

        public float MaxFuel { get; set; }
        public float Fuel { get; set; }
        public float MaxOil { get; set; }
        public float Oil { get; set; }
        public float Mileage { get; set; }
        public int PlateNumber { get; set; }
        public string PlateText { get; set; }

        public bool IsSpawned { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBlocked { get; set; }

        public IVehicle VehicleHandle { get; set; }
        public bool IsJobVehicle { get; set; }
    }
}
