using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public int Owner { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short Dimension { get; set; }
        public float Fuel { get; set; }
        public float Oil { get; set; }
        public bool IsJobVehicle { get; set; }

        public IVehicle VehicleHandle { get; set; }
    }
}
