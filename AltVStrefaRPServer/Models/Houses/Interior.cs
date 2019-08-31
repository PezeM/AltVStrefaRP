using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.Housing;

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
        
        /// <summary>
        /// Collection of flats using this interior
        /// </summary>
        public ICollection<Flat> Flats { get; private set; }
        
        public IColShape Colshape { get; set; }

        public Interior(string name, float x, float y, float z)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            
            Colshape = Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);

            // Interior exit colshape 
            Alt.OnColShape += (shape, entity, state) =>
            {
                if (shape != Colshape) return;
                if (!(entity is IStrefaPlayer player)) return;

                player.Emit("showInteriorExitMenu", state);
            };
            
            // Create marker where player can access house inventory
        }
        
        public Position GetPosition() => new Position(X, Y, Z);

        public async Task DeleteInteriorAsync(IInteriorDatabaseService interiorDatabaseService)
        {
            Colshape.Remove();
            await interiorDatabaseService.RemoveInteriorAsync(this);
        }
    }
}