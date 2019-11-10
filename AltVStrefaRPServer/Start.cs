using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Handlers;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Modules.Core.Colshapes;
using AltVStrefaRPServer.Modules.HousingModule;
using AltVStrefaRPServer.Modules.Tests;
using Newtonsoft.Json;

namespace AltVStrefaRPServer
{
    public class Start : AsyncResource
    {
        private IVehiclesManager _vehiclesManager;
        private IVehicleSpawnService _vehicleSpawnService;
        private SerializatorTest _serializatorTest;
        private ILogger<Start> _logger;
        
        private Startup _startup;

        public override void OnStart()
        {
            _startup = new Startup();
            _logger = _startup.ServiceProvider.GetService<ILogger<Start>>();
            _logger.LogInformation("Starting StrefaRP...");

            AltAsync.OnConsoleCommand += OnConsoleCommandAsync;
            AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicleAsync;
            Alt.OnPlayerEvent += OnOnPlayerEvent;

            NetworkingManager.Instance.InitializeNetworkingManager(AppSettings.Current);
            var playerConnectEvent = _startup.ServiceProvider.GetService<PlayerConnect>();
            var playerDiconnectEvent = _startup.ServiceProvider.GetService<PlayerDisconnect>();
            var vehicleHandler = _startup.ServiceProvider.GetService<VehicleHandler>();
            var vehicleShopsHandler = _startup.ServiceProvider.GetService<VehicleShopsHandler>();
            var bankHandler = _startup.ServiceProvider.GetServices<BankHandler>();
            var sittingHandler = _startup.ServiceProvider.GetService<SittingHandler>();
            var thrashBinsHandler = _startup.ServiceProvider.GetService<TrashbinsController>();
            var temporaryChatHandler = _startup.ServiceProvider.GetService<TemporaryChatHandler>();
            var timeManager = _startup.ServiceProvider.GetService<TimeController>();
            var objectSync = _startup.ServiceProvider.GetService<ObjectSync>();
            _vehicleSpawnService = _startup.ServiceProvider.GetService<IVehicleSpawnService>();

            var adminMenuHandler = _startup.ServiceProvider.GetService<AdminMenuHandler>();
            _vehiclesManager = _startup.ServiceProvider.GetService<IVehiclesManager>();
            //var vehicleMileageHandler = _startup.ServiceProvider.GetService<VehicleMileageHandler>();
            var vehicleShopManager = _startup.ServiceProvider.GetService<VehicleShopsManager>();
            var businessesManager = _startup.ServiceProvider.GetService<BusinessesManager>();
            var businessHandler = _startup.ServiceProvider.GetServices<BusinessHandler>();
            var characterCreator = _startup.ServiceProvider.GetService<CharacterCreator>();
            var adminCommands = _startup.ServiceProvider.GetService<AdminCommands>();
            var bankAccountsManager = _startup.ServiceProvider.GetServices<BankAccountManager>();
            var inventoryManager = _startup.ServiceProvider.GetService<InventoriesManager>();
            var inventoryHandler = _startup.ServiceProvider.GetService<InventoryHandler>();
            // Fractions
            var fractionManager = _startup.ServiceProvider.GetService<FractionsManager>();
            var fractionHandler = _startup.ServiceProvider.GetService<FractionHandler>();
            var townHallFractionHandler = _startup.ServiceProvider.GetService<TownHallFractionHandler>();
            // Housing
            var housesManager = _startup.ServiceProvider.GetService<HousesManager>();
            var interiorsManager = _startup.ServiceProvider.GetService<InteriorsManager>();
            var houseHandler = _startup.ServiceProvider.GetService<HouseHandler>();
            // Tests
            var trainController = _startup.ServiceProvider.GetService<TrainController>();

            Alt.On<IPlayer, ulong>("bigNumber", (player, number) =>
            {
                Alt.Log($"ULONG BIGNUMBER VALUE: {number.ToString()}");
            });

            Alt.On<IPlayer, long>("bigNumber", (player, number) =>
            {
                Alt.Log($"LONG BIGNUMBER VALUE: {number.ToString()}");
            });

            Alt.OnClient("discordOAuth2Result", (player, data) =>
            {
                Alt.Log("Some test");
                foreach (var item in data)
                {
                    Alt.Log($"AuthResult data: {item}");
                    if (item is Dictionary<string, object> dataDictionary)
                    {
                        Alt.Log("Its dictionary");
                        foreach (var (key, value) in dataDictionary)
                        {
                            Alt.Log($"Key: {key} Value: {value}");
                        }
                    }
                }
            });

            Alt.On<IPlayer>("TattooShop-GetPlayerTattoos", (player) => {
                player.Emit("TattooShop-OpenShop", new int[5]);
            });
        }


        public class TestOAuth
        {
            public string Token { get; set; }
            public string Expires { get; set; }
            public string Scopes { get; set; }
        }

        private void OnOnPlayerEvent(IPlayer player, string eventName, object[] args)
        {
            _logger.LogDebug("Event {eventName} triggered for player {playerName} with {argsLength} args", eventName, player.Name, args.Length);
        }

        private Task OnPlayerEnterVehicleAsync(IVehicle vehicle, IPlayer player, byte seat)
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
                Alt.Log(ex.ToString());
            }

            return Task.CompletedTask;
        }

        private async Task OnConsoleCommandAsync(string name, string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            switch (name)
            {
                case "stop":
                {
                    var resource = args[0];
                    if (resource.IsNullOrEmpty()) return;

                    Alt.Log($"Stopping resource: {resource}.");
                    break;
                }
                case "vehicle":
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
                    break;
                }
                case "spawn":
                {
                    if (!int.TryParse(args[0], out var id))
                    {
                        Alt.Log($"Wrong vehicle id for command {name}");
                        return;
                    }
                    SpawnVehicleComand(id);
                    break;
                }
                case "testStreamer":
                {
                    var startTime = Time.GetTimestampMs();
                    _logger.LogInformation("Started creating test objects.");
                    var idGenerator = new IdGenerator();
                    NetworkingManager.Instance.RemoveAllNetworkingEntities();
                    _logger.LogInformation($"Deleted all networking entities in {Time.GetElapsedTime(startTime)}ms.");

                    var offset = 10;
                    if (args != null && args.Length > 0)
                    {
                        int.TryParse(args[0], out offset);
                    }

                    string propModel = "imp_prop_ship_01a";
                        //string propModel = "prop_metal_plates01";

                    var newBaseItem = new DrinkItem("picie", 1, propModel, 10);

                    for (int i = -4000; i <= 4000; i += offset)
                    {
                        for (int y = -4000; y <= 4000; y += offset)
                        {
                            NetworkingManager.Instance.AddNewDroppedItem(new DroppedItem
                            {
                                Id = idGenerator.GetNextId(),
                                Count = 1,
                                Model = propModel,
                                X = i,
                                Y = y,
                                Z = 500,
                                Item = newBaseItem
                            }, 40);
                        }
                    }

                    _logger.LogInformation(
                        $"Created {NetworkingManager.Instance.GetAllEntitiesCount()} entities in {Time.GetElapsedTime(startTime)}ms.");
                    break;
                }
            }
            stopwatch.Stop();
            Alt.Log($"Executed console command in {stopwatch.Elapsed}");
        }

        public async Task VehicleCommandAsync(string vehicleModel, IPlayer player)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            var vehicle = await _vehiclesManager.CreateVehicleAsync(vehicleModel, player.Position, player.Rotation,
                player.Dimension, character.Id, OwnerType.None).ConfigureAwait(false);
            await _vehicleSpawnService.SpawnVehicleAsync(vehicle);
        }

        public void SpawnVehicleComand(int vehicleId)
        {
            if (!_vehiclesManager.TryGetVehicleModel(vehicleId, out var vehicle))
            {
                Alt.Log($"Didn't found vehicle with ID {vehicleId}");
                return;
            }
            _vehicleSpawnService.SpawnVehicle(vehicle);
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new CustomVehicleFactory();
        }

        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new StrefaPlayerFactory();
        }

        public override IBaseObjectFactory<IColShape> GetColShapeFactory()
        {
            return new StrefaColshapeFactory();
        }

        public override void OnStop()
        {
            Alt.Log($"Stopped resource {GetType().Namespace}");
        }

        public void Test()
        {
            _ = Task.Run(() =>
              {
                  _serializatorTest = _startup.ServiceProvider.GetService<SerializatorTest>();
                  _serializatorTest.ConvertToJson(_serializatorTest.TestObject);
                  _serializatorTest.ConvertToMessagePack(_serializatorTest.TestObject);
              });
        }
    }
}