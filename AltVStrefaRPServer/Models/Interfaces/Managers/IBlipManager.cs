using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IBlipManager : IManager<BlipWrapper>
    {
        IMValueBaseAdapter BlipAdapter { get; set; }

        HashSet<IBlipWrapper> GetBlipsList();
        IBlipWrapper CreateBlip(string blipName, int blipSprite, int blipColor, Position position, int blipType = 3);
        bool Remove(IBlipWrapper blip);
        bool Remove(int id);
    }
}