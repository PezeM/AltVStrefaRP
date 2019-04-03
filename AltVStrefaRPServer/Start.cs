using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Character;
using AltVStrefaRPServer.Modules.Character.Customization;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services.Vehicles;
using Microsoft.Extensions.DependencyInjection;

namespace AltVStrefaRPServer
{
    public class Start : AsyncResource
    {
        private VehicleManager vehicleManager;
        protected Startup Startup;
        public override void OnStart()
        {
            Alt.Server.LogInfo("Starting AltVTestResource...");

            AltAsync.OnConsoleCommand += OnConsoleCommand;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnPlayerEvent += AltAsyncOnOnPlayerEvent;

            Startup = new Startup();
            var playerConnectEvent = Startup.ServiceProvider.GetService<PlayerConnect>();
            var playerDiconnectEvent = Startup.ServiceProvider.GetService<PlayerDisconnect>();
            var vehicleHandler = Startup.ServiceProvider.GetService<VehicleHandler>();
            var characterCreator = Startup.ServiceProvider.GetService<CharacterCreator>();
            var vehicleLoader = Startup.ServiceProvider.GetService<VehicleManagerService>();
            vehicleManager = Startup.ServiceProvider.GetService<VehicleManager>();

            var sitting = new Sitting();

            // For now not working on windows
            //var chat = new ChatHandler();
            //chat.RegisterCommand("test", (player, strings) =>
            //{
            //    Alt.Log($"{player.Id} triggered command test");
            //});
        }

        private Task AltAsyncOnOnPlayerEvent(IPlayer player, string eventname, object[] args)
        {
            AltAsync.Log($"{eventname} event triggered for {player.Name}");
            return Task.CompletedTask;
        }

        private async Task OnPlayerEnterVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
        {
            try
            {
                var myVehicle = vehicle as MyVehicle;
                if (myVehicle == null) Alt.Log("Pojazd nie jest customowym typem IMyVehicle.");

                Alt.Log($"Pojazd jest customowym typem IMyVehicle. Paliwo {myVehicle.Fuel} Olej {myVehicle.Oil}");

            }
            catch (Exception ex)
            {
                Alt.Log(ex.ToString());
            }
        }

        private async Task OnConsoleCommand(string name, string[] args)
        {
            Alt.Log($"[CONSOLE COMMAND] Name: {name} args: {args}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (args.Length < 1) return;
            if (name == "stop")
            {
                var resource = args[0];
                if (string.IsNullOrEmpty(resource)) return;

                Alt.Log($"Stopping resource: {resource}.");
            }
            else if (name == "vehicle")
            {
                var model = args[0];
                if (string.IsNullOrEmpty(model)) return;

                var player = Alt.GetAllPlayers().FirstOrDefault();
                if (player == null)
                {
                    Alt.Log($"Not found any player in the game");
                    return;
                }

                await VehicleCommandAsync(model, player).ConfigureAwait(false);
            }
            else if (name == "spawn")
            {
                if (!int.TryParse(args[0], out int id))
                {
                    Alt.Log($"Wrong vehicle id for command {name}");
                    return;
                }
                SpawnVehicleComand(id);
            }
            stopwatch.Stop();
            Alt.Log($"Executed console command in {stopwatch.Elapsed}");
        }

        public async Task VehicleCommandAsync(string vehicleModel, IPlayer player)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            var vehicle = await vehicleManager.CreateVehicleAsync(vehicleModel, player.Position, player.HeadRotation.pitch,
                player.Dimension, character.Id).ConfigureAwait(false);
            vehicleManager.SpawnVehicle(vehicle.Id);
        }

        public void SpawnVehicleComand(int vehicleId)
        {
            vehicleManager.SpawnVehicle(vehicleId);
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new MyVehicleFactory();
        }

        public override void OnStop()
        {
            Alt.Log($"Stopped resource {GetType().Namespace}");
        }
    }
}
