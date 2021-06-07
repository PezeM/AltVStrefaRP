using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Tests
{
    public class TrainController
    {
        private ILogger<TrainController> _logger;
        private IVehicle _train;

        public TrainController(ILogger<TrainController> logger)
        {
            _logger = logger;
            CreateTrain();
            Alt.On<IPlayer>("Train-TestSpawn", OnTestSpawn);
        }

        private void OnTestSpawn(IPlayer player)
        {
            _logger.LogInformation("Player with ID({playerId}) wanted to test train", player.Id);
            player.Emit("Train-TestCreate", _train);
        }

        public void CreateTrain()
        {
            _train = Alt.CreateVehicle(VehicleModel.Freight, new Position(2533.0f, 2833.0f, 38.0f),
                new Rotation(0, 0, 0));
            _logger.LogInformation("Created train with ID({trainId}) at position {trainPosition}", _train.Id, _train.Position);
        }
    }
}
