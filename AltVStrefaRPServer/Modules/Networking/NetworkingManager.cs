using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;
using Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AltVStrefaRPServer.Models.Core;
using Serilog;

namespace AltVStrefaRPServer.Modules.Networking
{
    public sealed class NetworkingManager : INetworkingManager
    {
        private static readonly Lazy<NetworkingManager> lazy = new Lazy<NetworkingManager>(() => new NetworkingManager());
        public static NetworkingManager Instance => lazy.Value;
        private ConcurrentDictionary<ulong, INetworkingEntity> _entities;

        private NetworkingManager()
        {
            _entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
        }

        public void InitializeNetworkingManager(AppSettings appSettings)
        {
            //ConfigureAltNetworking(appSettings);

            //AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
            //AltNetworking.OnEntityStreamOut = OnEntityStreamOut;
        }

        public int GetAllEntitiesCount() => _entities.Count;

        public bool TryGetNetworkingEntity(int networkignEntityId, out INetworkingEntity networkingEntity)
            => _entities.TryGetValue((ulong)networkignEntityId, out networkingEntity);

        public bool DoesNetworkingEntityExists(int networkingEntityId) => _entities.ContainsKey((ulong)networkingEntityId);

        public bool RemoveNetworkingEntity(int netorkingItemId)
        {
            if (!_entities.TryGetValue((ulong)netorkingItemId, out var networkingEntity)) return false;
            networkingEntity.Remove();
            return true;
        }

        public bool RemoveNetworkingEntity(INetworkingEntity networkingEntity)
        {
            if (networkingEntity == null) return false;
            networkingEntity.Remove();
            return true;
        }

        public void RemoveAllNetworkingEntities()
        {
            foreach (var keyValuePair in _entities)
            {
                keyValuePair.Value.Remove();
            }

            _entities.Clear();
        }

        public INetworkingEntity AddNewDroppedItem(DroppedItem droppedItem, int streamingRange = 50, int dimension = 0)
        {
            //var networkingEntity = AltNetworking.CreateEntity(new Position { X = droppedItem.X, Y = droppedItem.Y, Z = droppedItem.Z },
            //    dimension, streamingRange, new Dictionary<string, object>
            //    {
            //        { "entityType", (long)NetworkingEntityTypes.Item },
            //        { "id", droppedItem.Id },
            //        { "name", droppedItem.Name },
            //        { "count", droppedItem.Count },
            //        { "model", droppedItem.Model }
            //    });
            //_entities.TryAdd(networkingEntity.Id, networkingEntity);
            //return networkingEntity;
            return null;
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

        public void AddNewMarker(Marker marker)
        {
            //var networkingEntity = AltNetworking.CreateEntity(new Position{X = marker.X, Y = marker.Y, Z = marker.Z}, marker.Dimension, marker.Range, 
            //    new Dictionary<string, object>
            //{
            //    { "entityType", (int)NetworkingEntityTypes.Marker },
            //    { "type", marker.Type },
            //    { "scaleX", marker.ScaleX },
            //    { "scaleY", marker.ScaleY },
            //    { "scaleZ", marker.ScaleZ },
            //    { "red", marker.Red },
            //    { "green", marker.Green },
            //    { "blue", marker.Blue },
            //    { "alpha", marker.Alpha },
            //});
            //_entities.TryAdd(networkingEntity.Id, networkingEntity);
            //marker.NetworkingEntity = networkingEntity;
        }

        private void OnEntityStreamOut(INetworkingEntity entity, INetworkingClient client)
        {
            //Log.ForContext<NetworkingManager>().Debug("Entity streamed out {networkingEntityId}", entity.Id);
        }

        private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
        {
            //Log.ForContext<NetworkingManager>().Debug("Entity streamed in {networkingEntityId}", entity.Id);
        }

        private void ConfigureAltNetworking(AppSettings appSettings)
        {
            //try
            //{
            //    AltNetworking.Configure(options =>
            //    {
            //        options.Port = appSettings.ServerConfig.NetworkingManagerConfig.Port;
            //        options.Ip = appSettings.ServerConfig.NetworkingManagerConfig.Ip;
            //    });
            //}
            //catch (Exception e)
            //{
            //    Log.ForContext<NetworkingManager>().Fatal(e, "Error in networking manager. Couldn't configure AltNetworking module.");
            //    throw;
            //}
        }
    }
}
