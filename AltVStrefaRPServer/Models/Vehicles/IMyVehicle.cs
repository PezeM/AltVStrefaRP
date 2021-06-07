using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models.Vehicles
{
    public interface IMyVehicle : IVehicle
    {
        int DatabaseId { get; set; }
        float Fuel { get; set; }
        float Oil { get; set; }
        float Mileage { get; set; }
        string Owner { get; set; }
        string CustomData { get; set; }
    }
}
