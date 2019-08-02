﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using AltV.Net;
using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using Timer = System.Timers.Timer;
using Entity;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Networking
{
    public class NetworkingManager : INetworkingManager
    {
        private ConcurrentDictionary<ulong, INetworkingEntity> _entities;
        private readonly Random _rng;
        private readonly ILogger<NetworkingManager> _logger;
        private Timer _testTimer;

        public NetworkingManager(AppSettings appSettings, ILogger<NetworkingManager> logger)
        {
            _rng = new Random();
            _logger = logger;
            _entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
            ConfigureAltNetworking(appSettings);

            AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
            AltNetworking.OnEntityStreamOut = OnEntityStreamOut;

            CreateRandomItems();
            CreateRandomPeds();
        }

        public bool TryGetNetworkingEntity(int networkignEntityId, out INetworkingEntity networkingEntity)
            => _entities.TryGetValue((ulong)networkignEntityId, out networkingEntity);

        public bool DoesNetworkingEntityExists(int networkingEntityId) => _entities.ContainsKey((ulong)networkingEntityId);

        public bool RemoveNetworkingEntity(int networkingItemId) => _entities.TryRemove((ulong)networkingItemId, out _);

        public INetworkingEntity AddNewDroppedItem(DroppedItem droppedItem, int streamingRange = 50, int dimension = 0)
        {
            var networkingEntity = AltNetworking.CreateEntity(new Position { X = droppedItem.X, Y = droppedItem.Y, Z = droppedItem.Z },
                dimension, streamingRange, new Dictionary<string, object>
                {
                    { "entityType", (long)NetworkingEntityTypes.Item },
                    { "id", droppedItem.Id },
                    { "name", droppedItem.Name },
                    { "count", droppedItem.Count },
                    { "model", droppedItem.Model }
                });
            _entities.TryAdd(networkingEntity.Id, networkingEntity);
            return networkingEntity;
        }

        public void DescreaseDroppedItemQuantity(int networkingItemId, int itemsToRemove)
        {
            if (!TryGetNetworkingEntity(networkingItemId, out var networkingEntity)) return;
            if (!networkingEntity.GetData("count", out long quantity)) return;
            quantity -= itemsToRemove;
            if (quantity <= 0)
            {
                _entities.TryRemove(networkingEntity.Id, out _);
                networkingEntity.Remove();
            }
            else
            {
                networkingEntity.SetData("count", quantity);
            }
        }

        private void OnEntityStreamOut(INetworkingEntity entity, INetworkingClient client)
        {
            _logger.LogDebug("Entity streamed out {networkingEntityId}", entity.Id);
            //DisplayInfo(entity, client);
        }

        private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
        {
            _logger.LogDebug("Entity streamed in {networkingEntityId}", entity.Id);
            //DisplayInfo(entity, client);
        }

        private void CreateRandomItems()
        {
            for (int i = 0; i < 2; i++)
            {
                AltNetworking.CreateEntity(new Position { X = -82 + (i * 15), Y = -109 + (i * 15), Z = 62 }, 0, 100, GetRandomItemData());
            }
        }

        private void CreateRandomPeds()
        {
            for (int i = 0; i < 2; i++)
            {
                AltNetworking.CreateEntity(new Position { X = -82 + (i * 15), Y = -109 + (i * 15), Z = 62 }, 0, 100, GetPedRandomData());
            }
        }

        private void ConfigureAltNetworking(AppSettings appSettings)
        {
            try
            {
                AltNetworking.Configure(options =>
                {
                    options.Port = appSettings.ServerConfig.NetworkingManagerConfig.Port;
                    options.Ip = appSettings.ServerConfig.NetworkingManagerConfig.Ip;
                });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error in networking manager. Couldn't configure AltNetworking module.");
                throw;
            }
        }

        private void SetupTimer()
        {
            _testTimer = new Timer(10000);
            _testTimer.Elapsed += OnTimer;
            _testTimer.AutoReset = true;
            _testTimer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                var player = Alt.GetAllPlayers().FirstOrDefault();
                if (player == null) return;
                var randomPosition = GenerateRandomPosition(player.Position);
                var newEntity = AltNetworking.CreateEntity(new Position { X = player.Position.X, Y = player.Position.Y, Z = player.Position.Z, },
                    0, _rng.Next(100, 150), GetRandomItemData());
                _entities.TryAdd(newEntity.Id, newEntity);
                Alt.Log($"Created entity {newEntity.Id} dim {newEntity.Dimension} on thread {Thread.CurrentThread.ManagedThreadId}");
                //if (Entities.Count >= 5)
                //{
                //    _testTimer.Stop();
                //    return;
                //}
            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void DisplayInfo(INetworkingEntity entity, INetworkingClient client)
        {
            var stringBuilder = new StringBuilder();
            if (entity.StreamedInClients.Count > 0)
            {
                stringBuilder.AppendLine($"Streamed clients: ");
            }
            foreach (var entityStreamedInClient in entity.StreamedInClients)
            {
                stringBuilder.AppendLine($"Token: {entityStreamedInClient.Token} Exists: {entityStreamedInClient.Exists} " +
                                         $"LocalEndPoint: {entityStreamedInClient.WebSocket.LocalEndPoint} RemoteEndPoint: {entityStreamedInClient.WebSocket.RemoteEndPoint}");
            }
            stringBuilder.AppendLine($"Entity data: Range {entity.Range} PositionSize {entity.Position.CalculateSize()}");
            Alt.Log(stringBuilder.ToString());
        }

        private AltV.Net.Data.Position GenerateRandomPosition(AltV.Net.Data.Position position)
        {
            var newPosition = new AltV.Net.Data.Position
            {
                X = position.X + _rng.Next(1, 10),
                Y = position.Y + _rng.Next(1, 10),
                Z = position.Z + _rng.Next(1, 10)
            };
            return newPosition;
        }

        private Dictionary<string, object> GenerateRandomData()
        {
            var testData = new Dictionary<string, object>
            {
                { "someData", (long)_rng.Next(1000,3000)},
                { "propModel", "somePropModel" },
                { "quantity", (long)_rng.Next(1, 10)},
                { "canPickup", true}
            };
            return testData;
        }

        private Dictionary<string, object> GetPedRandomData()
        {
            var testData = new Dictionary<string, object>
            {
                { "entityType", (long)NetworkingEntityTypes.Ped },
                { "pedType",  (long)PedTypes.BankPed }
            };
            return testData;
        }

        private Dictionary<string, object> GetRandomItemData()
        {
            return new Dictionary<string, object>
            {
                { "entityType", (long)NetworkingEntityTypes.Item },
                { "id", 2 },
                { "name", "Jakiś itemek" },
                { "count", 15 },
                { "model", "prop_bodyarmour_04" }
            };
        }
    }
}
