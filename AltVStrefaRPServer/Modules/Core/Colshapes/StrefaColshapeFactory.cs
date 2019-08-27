using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Modules.Core.Colshapes
{
    public class StrefaColshapeFactory : IBaseObjectFactory<IColShape>
    {
        public IColShape Create(IntPtr baseObjectPointer)
        {
            return new StrefaColshape(baseObjectPointer);
        }
    }
}