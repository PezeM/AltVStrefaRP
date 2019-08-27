using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models.Core
{
    public interface IStrefaColshape : IColShape
    {
        int HouseId { get; set; }
    }
}