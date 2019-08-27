using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : IPosition
    {
        public int Id { get; set; }
        public Character Owner { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Price { get; set; }
        public bool Locked { get; set; }
        
        // Don't know if its needed and will do M-1 
        public Interior Interior { get; set; }

        public IColShape Colshape { get; private set; }
        
        public House()
        {
            Colshape = Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.SetData(MetaData.COLSHAPE_HOUSE_ID, Id);
        }
        
        public Position GetPosition() => new Position(X, Y, Z);
    }
}
