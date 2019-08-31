using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.Housing;

namespace AltVStrefaRPServer.Models.Houses
{
    public interface IInterior : IPosition
    {
        /// <summary>
        /// Database id of the interior
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Name of the interior
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Collection of flats using this interior
        /// </summary>
        ICollection<Flat> Flats { get; }

        IColShape Colshape { get; set; }
        Task DeleteInteriorAsync(IInteriorDatabaseService interiorDatabaseService);
    }
}