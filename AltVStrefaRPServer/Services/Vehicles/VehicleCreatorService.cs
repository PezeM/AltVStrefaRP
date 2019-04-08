using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Vehicles
{
    public class VehicleCreatorService : IVehicleCreatorService
    {
        private ServerContext _serverContext;

        public VehicleCreatorService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public async Task SaveVehicleToDatabaseAsync(VehicleModel vehicle)
        {
            await _serverContext.Vehicles.AddAsync(vehicle).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public void SaveVehicleToDatabase(VehicleModel vehicle)
        {
            _serverContext.Vehicles.Add(vehicle);
            _serverContext.SaveChanges();
        }

        /// <summary>
        /// Creates <see cref="VehicleModel"/> with default values
        /// </summary>
        /// <param name="vehicleModel">todo: describe vehicleModel parameter on CreateVehicle</param>
        /// <param name="position">todo: describe position parameter on CreateVehicle</param>
        /// <param name="heading">todo: describe heading parameter on CreateVehicle</param>
        /// <param name="dimension">todo: describe dimension parameter on CreateVehicle</param>
        /// <param name="ownerId">todo: describe ownerId parameter on CreateVehicle</param>
        /// <returns></returns>
        public VehicleModel CreateVehicle(string vehicleModel, Position position, float heading, short dimension, int ownerId, OwnerType ownerType)
        {
            return new VehicleModel
            {
                Owner = ownerId,
                Model = vehicleModel,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Dimension = dimension,
                Heading = heading,
                OwnerType = ownerType,
                PlateNumber = 0,
                PlateText = "", // Change it to unique plate text
                // Temporary values
                MaxFuel = 50.0f,
                Fuel = 50.0f,
                MaxOil = 10.0f,
                Oil = 5.0f,
                Mileage = 0.0f,
                IsBlocked = false,
                IsJobVehicle = false,
                IsLocked = false,
                IsSpawned = false
            };
        }
    }
}
