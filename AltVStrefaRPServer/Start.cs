using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Character.Customization;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Vehicle;
using Microsoft.Extensions.DependencyInjection;

namespace AltVStrefaRPServer
{
    public class Start : AsyncResource
    {
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
            var characterCreator = Startup.ServiceProvider.GetService<CharacterCreator>();
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

                try
                {
                    var player = Alt.GetAllPlayers().FirstOrDefault();
                    if (player == null)
                    {
                        Alt.Log($"Not found any player in the game");
                        return;
                    }

                    Alt.Log($"Creating vehicle at position: {player.Position}.");
                    var vehicle = Alt.CreateVehicle(model, player.Position, float.MinValue);
                    await vehicle.SetPrimaryColorRgbAsync(new Rgba(0, 0, 0, 255)).ConfigureAwait(false);
                    await vehicle.SetSecondaryColorRgbAsync(new Rgba(255, 255, 255, 255)).ConfigureAwait(false);
                    await vehicle.SetNumberplateTextAsync("StrefaRP").ConfigureAwait(false);
                    await vehicle.SetNumberplateIndexAsync(3).ConfigureAwait(false);
                    await vehicle.SetDimensionAsync(player.Dimension).ConfigureAwait(false);
                    Alt.Log($"Created vehicle {vehicle}");
                }
                catch (Exception e)
                {
                    Alt.Log($"Couldn't create a vehicle by command line. {e}");
                }
            }
            stopwatch.Stop();
            Alt.Log($"Executed console command in {stopwatch.Elapsed}");
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
