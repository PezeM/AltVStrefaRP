using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Houses
{
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
        
        public IColShape Colshape { get; set; }

        public Interior()
        {
            Colshape = Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);

            // Interior exit colshape 
            Alt.OnColShape += (shape, entity, state) =>
            {
                if (shape != Colshape || !state) return;
                if (!(entity is IStrefaPlayer player)) return;

                player.Emit("showInteriorExitMenu", state);
            };
        }
        
        public Position GetPosition() => new Position(X, Y, Z);

    }
}