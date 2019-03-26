using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public interface IMyVehicle : IVehicle
    {
        float Fuel { get; set; }
        float Oil { get; set; }
        string Owner { get; set; }
    }
}
