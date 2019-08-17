using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;

namespace AltVStrefaRPServer.Models.Vehicles
{
    public class MyVehicle : Vehicle, IMyVehicle
    {
        public int DatabaseId { get; set; }
        public float Fuel { get; set; } = 50f;

        public float Oil { get; set; } = 50f;

        public string Owner { get; set; }

        public string CustomData { get; set; }

        public MyVehicle(uint model, Position position, Rotation rotation) : base(model, position, rotation)
        {
            CustomData = "By constructor";
        }

        public MyVehicle(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            Owner = id.ToString();
            CustomData = "By entity factory";
        }
    }
}
