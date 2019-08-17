using AltV.Net;
using AltV.Net.Data;

namespace AltVStrefaRPServer.Models.Core
{
    public interface IBlipWrapper
    {
        int Id { get; set; }
        string Name { get; set; }
        int Sprite { get; set; }
        int Color { get; set; }
        int Type { get; set; }
        Position Position { get; set; }

        void DestroyBlip();
    }
}