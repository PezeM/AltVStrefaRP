using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Houses
{
    public abstract class HouseBuilding : IPosition
    {
        public int Id { get; private set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public int Price { get; set; }
        
        public IStrefaColshape Colshape { get; protected set; }

        public Position GetPosition() => new Position(X, Y, Z);
        
        public abstract void InitializeHouse();
    }
}