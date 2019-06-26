using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Helpers
{
    public static class BlipHelper
    {
        public static IBlip CreateBlip(byte blipType, Position position)
        {
            var blip = Alt.CreateBlip((BlipType)blipType, position);
            return blip;
        }
    }
}
