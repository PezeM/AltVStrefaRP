using AltV.Net;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Houses.Interfaces
{
    public interface IHouseBuilding : IPosition, IWritable
    {
        int Id { get; }
        int Price { get; set; }
        IStrefaColshape Colshape { get; }
        IMarker Marker { get; }
        void InitializeHouseBuilding();
    }
}