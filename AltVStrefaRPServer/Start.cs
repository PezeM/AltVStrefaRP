using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Modules.Admin;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule.Customization;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Money;
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
            Alt.OnConsoleCommand += OnConsoleCommand2;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            AltAsync.OnPlayerEvent += AltAsyncOnOnPlayerEvent;

            Startup = new Startup();
            var playerConnectEvent = Startup.ServiceProvider.GetService<PlayerConnect>();
            var playerDiconnectEvent = Startup.ServiceProvider.GetService<PlayerDisconnect>();
            var vehicleHandler = Startup.ServiceProvider.GetService<VehicleHandler>();
            var bankHandler = Startup.ServiceProvider.GetServices<BankHandler>();
            var sittingHandler = Startup.ServiceProvider.GetService<SittingHandler>();
            var thrashBinsHandler = Startup.ServiceProvider.GetService<ThrashBinsHandler>();
            var temporaryChatHandler = Startup.ServiceProvider.GetService<TemporaryChatHandler>();
            var timeManager = Startup.ServiceProvider.GetService<TimeManager>();
            var objectSync = Startup.ServiceProvider.GetService<ObjectSync>();

            vehicleManager = Startup.ServiceProvider.GetService<VehicleManager>();
            var businessesManager = Startup.ServiceProvider.GetService<BusinessManager>();
            var businessHandler = Startup.ServiceProvider.GetServices<BusinessHandler>();
            var characterCreator = Startup.ServiceProvider.GetService<CharacterCreator>();
            var vehicleLoader = Startup.ServiceProvider.GetService<VehicleDatabaseService>();
            var adminCommands = Startup.ServiceProvider.GetService<AdminCommands>();
            var bankAccountsManager = Startup.ServiceProvider.GetServices<BankAccountManager>();
            // For now not working on windows
            //var chat = new ChatHandler();
            //chat.RegisterCommand("test", (player, strings) =>
            //{
            //    Alt.Log($"{player.Id} triggered command test");
            //});
        }

        private void OnConsoleCommand2(string name, string[] args)
        {
            Alt.Log("Triggered console event");
        }

        private Task AltAsyncOnOnPlayerEvent(IPlayer player, string eventname, object[] args)
        {
            AltAsync.Log($"{eventname} event triggered for {player.Name} with {args.Length} args.");
            return Task.CompletedTask;
        }

        private Task OnPlayerEnterVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
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

            return Task.CompletedTask;
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
                if (resource.IsNullOrEmpty()) return;

                Alt.Log($"Stopping resource: {resource}.");
            }
            else if (name == "vehicle")
            {
                var model = args[0];
                if (model.IsNullOrEmpty()) return;

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
            var character = player.GetCharacter();
            if (character == null) return;

            var vehicle = await vehicleManager.CreateVehicleAsync(vehicleModel, player.Position, player.HeadRotation.Pitch,
                player.Dimension, character.Id, OwnerType.None).ConfigureAwait(false);
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
