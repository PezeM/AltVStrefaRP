using AltV.Net.Data;

namespace AltVStrefaRPServer.Models.Interfaces
{
    public interface IPosition
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
        Position GetPosition();
    }
}
