using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.Housing;

namespace AltVStrefaRPServer.Models.Houses.Interfaces
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

        ICollection<House> Houses { get; }
        ICollection<HotelRoom> HotelRooms { get; }

        IColShape Colshape { get; set; }
        Task DeleteInteriorAsync(IInteriorDatabaseService interiorDatabaseService);
    }
}