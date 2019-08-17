using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Vehicles;
using System;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class CustomVehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(IntPtr entityPointer, ushort id)
        {
            return new MyVehicle(entityPointer, id);
        }
    }
}
