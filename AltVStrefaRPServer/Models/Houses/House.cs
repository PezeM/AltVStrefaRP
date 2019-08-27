using AltV.Net;
using AltV.Net.Data;
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
        
        public Position GetPosition() => new Position(X, Y, Z);
    }
    
    public class Interior : IPosition
    {
        /// <summary>
        /// Database id of the interior
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the interior
        /// </summary>
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public Position GetPosition() => new Position(X, Y, Z);
    }    
}
