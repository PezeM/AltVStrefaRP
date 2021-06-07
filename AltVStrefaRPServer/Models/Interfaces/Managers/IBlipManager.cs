using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using AltV.Net.Elements.Args;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IBlipManager : IManager<BlipWrapper>
    {
        IMValueBaseAdapter BlipAdapter { get; set; }
        List<BlipWrapper> GetBlips();
        IBlipWrapper CreateBlip(string blipName, int blipSprite, int blipColor, Position position, float scale = 1);
        bool Remove(IBlipWrapper blip);
        bool Remove(int id);
    }
}