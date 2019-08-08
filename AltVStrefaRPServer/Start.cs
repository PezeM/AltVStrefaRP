using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Handlers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Vehicles;
using AltVStrefaRPServer.Modules.Admin;
using AltVStrefaRPServer.Modules.Businesses;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.CharacterModule.Customization;
using AltVStrefaRPServer.Modules.Chat;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Modules.Fractions;
using AltVStrefaRPServer.Modules.Inventory;
using AltVStrefaRPServer.Modules.Money;
using AltVStrefaRPServer.Modules.Networking;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer
{
    public class Start : AsyncResource
    {
        private IVehiclesManager _vehiclesManager;
        private IVehicleSpawnService _vehicleSpawnService;
        private SerializatorTest _serializatorTest;
        private ILogger<Start> _logger;

        protected Startup Startup;
        public override void OnStart ()
        {
            Startup = new Startup ();
            _logger = Startup.ServiceProvider.GetService<ILogger<Start>>();
            _logger.LogInformation("Starting StrefaRP...");

            AltAsync.OnConsoleCommand += OnConsoleCommand;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            Alt.OnPlayerEvent += OnOnPlayerEvent;

            var playerConnectEvent = Startup.ServiceProvider.GetService<PlayerConnect> ();
            var playerDiconnectEvent = Startup.ServiceProvider.GetService<PlayerDisconnect> ();
            var vehicleHandler = Startup.ServiceProvider.GetService<VehicleHandler>();
            var vehicleShopsHandler = Startup.ServiceProvider.GetService<VehicleShopsHandler> ();
            var bankHandler = Startup.ServiceProvider.GetServices<BankHandler> ();
            var sittingHandler = Startup.ServiceProvider.GetService<SittingHandler> ();
            var thrashBinsHandler = Startup.ServiceProvider.GetService<TrashbinsController> ();
            var temporaryChatHandler = Startup.ServiceProvider.GetService<TemporaryChatHandler> ();
            var timeManager = Startup.ServiceProvider.GetService<TimeController> ();
            var objectSync = Startup.ServiceProvider.GetService<ObjectSync> ();
            var networkingTest = Startup.ServiceProvider.GetService<NetworkingManager>();
            _vehicleSpawnService = Startup.ServiceProvider.GetService<IVehicleSpawnService> ();

            _vehiclesManager = Startup.ServiceProvider.GetService<IVehiclesManager> ();
            var vehicleShopManager = Startup.ServiceProvider.GetService<VehicleShopsManager> ();
            var businessesManager = Startup.ServiceProvider.GetService<BusinessesManager> ();
            var businessHandler = Startup.ServiceProvider.GetServices<BusinessHandler> ();
            var characterCreator = Startup.ServiceProvider.GetService<CharacterCreator> ();
            var adminCommands = Startup.ServiceProvider.GetService<AdminCommands> ();
            var bankAccountsManager = Startup.ServiceProvider.GetServices<BankAccountManager> ();
            var inventoryManager = Startup.ServiceProvider.GetService<InventoriesManager>();
            var inventoryHandler = Startup.ServiceProvider.GetService<InventoryHandler>();
            // Fractions
            var fractionManager = Startup.ServiceProvider.GetService<FractionsManager> ();
            var fractionHandler = Startup.ServiceProvider.GetService<FractionHandler> ();
            var townHallFractionHandler = Startup.ServiceProvider.GetService<TownHallFractionHandler>();

            Alt.On<IPlayer, ulong>("bigNumber", (player, number) =>
            {
                Alt.Log($"ULONG BIGNUMBER VALUE: {number}");
            });

            Alt.On<IPlayer, long>("bigNumber", (player, number) =>
            {
                Alt.Log($"LONG BIGNUMBER VALUE: {number}");
            });
        }

        private void OnOnPlayerEvent (IPlayer player, string eventName, object[] args)
        {
            _logger.LogDebug("Event {eventName} triggered for player {playerName} with {argsLength} args", eventName, player.Name, args.Length);
        }

        private Task OnPlayerEnterVehicleAsync (IVehicle vehicle, IPlayer player, byte seat)
        {
            try
            {
                if (vehicle is MyVehicle myVehicle)
                {
                    _logger.LogDebug($"Pojazd jest customowym typem IMyVehicle. Paliwo {myVehicle.Fuel} Olej {myVehicle.Oil} Data {myVehicle.CustomData}");
                }
            }
            catch (Exception ex)
            {
                Alt.Log (ex.ToString ());
            }

            return Task.CompletedTask;
        }

        private async Task OnConsoleCommand (string name, string[] args)
        {
            Alt.Log ($"[CONSOLE COMMAND] Name: {name} args: {args}");
            var stopwatch = new Stopwatch ();
            stopwatch.Start ();

            if (args.Length < 1) return;
            if (name == "stop")
            {
                var resource = args[0];
                if (resource.IsNullOrEmpty ()) return;

                Alt.Log ($"Stopping resource: {resource}.");
            }
            else if (name == "vehicle")
            {
                var model = args[0];
                if (model.IsNullOrEmpty ()) return;

                var player = Alt.GetAllPlayers ().FirstOrDefault ();
                if (player == null)
                {
                    Alt.Log ($"Not found any player in the game");
                    return;
                }

                await VehicleCommandAsync (model, player).ConfigureAwait (false);
            }
            else if (name == "spawn")
            {
                if (!int.TryParse (args[0], out int id))
                {
                    Alt.Log ($"Wrong vehicle id for command {name}");
                    return;
                }
                SpawnVehicleComand (id);
            }
            stopwatch.Stop ();
            Alt.Log ($"Executed console command in {stopwatch.Elapsed}");
        }

        public async Task VehicleCommandAsync (string vehicleModel, IPlayer player)
        {
            var character = player.GetCharacter ();
            if (character == null) return;

            var vehicle = await _vehiclesManager.CreateVehicleAsync (vehicleModel, player.Position, player.Rotation,
                player.Dimension, character.Id, OwnerType.None).ConfigureAwait (false);
            await _vehicleSpawnService.SpawnVehicleAsync (vehicle);
        }

        public void SpawnVehicleComand (int vehicleId)
        {
            if (!_vehiclesManager.TryGetVehicleModel(vehicleId, out VehicleModel vehicle))
            {
                Alt.Log($"Didn't found vehicle with ID {vehicleId}");
                return;
            }
            _vehicleSpawnService.SpawnVehicle (vehicle);
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory ()
        {
            return new CustomVehicleFactory ();
        }

        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new StrefaPlayerFactory();
        }

        public override void OnStop ()
        {
            Alt.Log ($"Stopped resource {GetType().Namespace}");
        }

        public void Test ()
        {
            _ = Task.Run(() =>
              {
                  _serializatorTest = Startup.ServiceProvider.GetService<SerializatorTest>();
                  _serializatorTest.ConvertToJson(_serializatorTest.TestObject);
                  _serializatorTest.ConvertToMessagePack(_serializatorTest.TestObject);
              });
        }
    }
}