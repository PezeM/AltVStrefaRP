using System;
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

namespace AltVStrefaRPServer.Modules.Networking
{
    public class NetworkingManager
    {
        public ConcurrentDictionary<ulong, INetworkingEntity> Entities { get; set; }
        private Random _rng;
        private Timer _testTimer;

        public NetworkingManager()
        {
            _rng = new Random();
            Entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
            AltNetworking.Configure(options =>
            {
                options.Port = 46429;
                options.Ip = "127.0.0.1";
            });
            Alt.Log("Initialized TestNetworking");
            var someEntity = AltNetworking.CreateEntity(new Position { X = -82, Y = -109, Z = 62 }, 0, 500, new Dictionary<string, object>
            {
                { "someData", 2331 },
            });
            Alt.Log($"Created entity with id: {someEntity.Id} and data 2331");
            Entities.TryAdd(someEntity.Id, someEntity);
            AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
            AltNetworking.OnEntityStreamOut = OnEntityStreamOut;
            //SetupTimer();

            CreateRandomItems();
            CreateRandomPeds();
        }

        public INetworkingEntity AddNewDroppedItem(DroppedItem droppedItem)
        {
            var networkingEntity = AltNetworking.CreateEntity(new Position {X = droppedItem.X, Y = droppedItem.Y, Z = droppedItem.Z}, 
                0, 50, new Dictionary<string, object>
                {
                    { "entityType", (long)NetworkingEntityTypes.Item },
                    { "id", droppedItem.Id },
                    { "name", droppedItem.Name },
                    { "count", droppedItem.Count },
                    { "model", droppedItem.Model }
                });
            Entities.TryAdd(networkingEntity.Id, networkingEntity);
            return networkingEntity;
        }

        private void CreateRandomItems()
        {
            for (int i = 0; i < 2; i++)
            {
                AltNetworking.CreateEntity(new Position {X = -82 + (i * 15), Y = -109 + (i * 15), Z = 62}, 0, 100, GetRandomItemData());
            }
        }

        private void CreateRandomPeds()
        {
            for (int i = 0; i < 2; i++)
            {
                AltNetworking.CreateEntity(new Position {X = -82 + (i * 15), Y = -109 + (i * 15), Z = 62}, 0, 100, GetPedRandomData());
            }
        }

        public void SetupTimer()
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
                Entities.TryAdd(newEntity.Id, newEntity);
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

        private void OnEntityStreamOut(INetworkingEntity entity, INetworkingClient client)
        {
            Alt.Log($"Entity streamed out {entity.Id}");
            //DisplayInfo(entity, client);
        }

        private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
        {
            Alt.Log($"Entity streamed in {entity.Id}");
            //DisplayInfo(entity, client);
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
