using AltV.Net;
using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;
using Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AltVStrefaRPServer.Modules.Networking
{
    public class NetworkingManager : INetworkingManager
    {
        private ConcurrentDictionary<ulong, INetworkingEntity> _entities;
        private readonly ILogger<NetworkingManager> _logger;

        public NetworkingManager(AppSettings appSettings, ILogger<NetworkingManager> logger)
        {
            _logger = logger;
            _entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
            ConfigureAltNetworking(appSettings);

            AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
            AltNetworking.OnEntityStreamOut = OnEntityStreamOut;
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
        }

        private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
        {
            _logger.LogDebug("Entity streamed in {networkingEntityId}", entity.Id);
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
    }
}
