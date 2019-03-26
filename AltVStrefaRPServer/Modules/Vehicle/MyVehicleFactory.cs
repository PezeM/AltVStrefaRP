using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class MyVehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(IntPtr entityPointer, ushort id)
        {
            return new MyVehicle(entityPointer, id);
        }
    }
}
