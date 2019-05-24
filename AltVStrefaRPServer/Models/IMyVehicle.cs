using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models
{
    public interface IMyVehicle : IVehicle
    {
        int DatabaseId { get; set; }
        float Fuel { get; set; }
        float Oil { get; set; }
        string Owner { get; set; }
        string CustomData { get; set; }
    }
}
