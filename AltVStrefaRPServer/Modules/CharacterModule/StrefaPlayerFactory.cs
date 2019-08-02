using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.CharacterModule
{
    public class StrefaPlayerFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(IntPtr entityPointer, ushort id)
        {
            return new StrefaPlayer(entityPointer, id);
        }
    }
}
