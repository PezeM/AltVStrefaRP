using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models.Core
{
    public interface IStrefaColshape : IColShape
    {
        bool HouseColshape { get; set; }
        int HouseId { get; set; }
    }
}