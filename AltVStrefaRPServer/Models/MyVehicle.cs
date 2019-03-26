using System;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public class MyVehicle : Vehicle, IMyVehicle
    {
        public float Fuel { get; set; }

        public float Oil { get; set; }

        public string Owner { get; set; }

        public MyVehicle(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            Fuel = 50f;
            Oil = 10f;
            Owner = string.Empty;
        }
    }
}
